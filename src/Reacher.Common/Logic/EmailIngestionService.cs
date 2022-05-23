using Microsoft.Extensions.Logging;

namespace Reacher.Common.Logic;

public class EmailIngestionService : IEmailIngestionService
{
    private readonly AppDbContext _db;
    private readonly IEmailContentRenderer _emailContentRenderer;
    private readonly ISendGridParser _sendGridParser;
    private readonly IStrikeFacade _strikeFacade;
    private readonly ISendGridFacade _sendGridFacade;
    private readonly IEmailContentService _emailContentService;
    private readonly IEmailForwardingService _emailForwardingService;

    public EmailIngestionService(AppDbContext db, IEmailContentRenderer emailContentRenderer, ISendGridParser sendGridParser,
        IStrikeFacade strikeFacade, ISendGridFacade sendGridFacade, IEmailContentService emailContentService, IEmailForwardingService emailForwardingService)
    {
        _db = db;
        _emailContentRenderer = emailContentRenderer;
        _sendGridParser = sendGridParser;
        _strikeFacade = strikeFacade;
        _sendGridFacade = sendGridFacade;
        _emailContentService = emailContentService;
        _emailForwardingService = emailForwardingService;
    }

    public async Task IngestEmail(Stream emailMessage)
    {
        using var emailStream = new MemoryStream();
        await emailMessage.CopyToAsync(emailStream);
        // 20mb limit
        emailStream.Position = 0;
        var emailId = Guid.NewGuid();
        var parsedEmail = await _sendGridParser.ParseSendGridInboundEmail(emailStream);
        emailStream.Position = 0;
        var incomingEmail = _sendGridParser.MapInboundEmailToDbEmail(parsedEmail);
        if (incomingEmail.ToEmailAddress == null)
        {
            return;
        }
        incomingEmail.Id = emailId;
        incomingEmail.ContentLength = emailStream.Length;
        _db.Emails.Add(incomingEmail);
        await _db.SaveChangesAsync();

        Guid? originalEmailId = null;
        var toEmailAddress = incomingEmail.ToEmailAddress;
        var splitEmail = toEmailAddress?.Split('+', '@');
        if (splitEmail?.Length == 3 && Guid.TryParse(splitEmail[1], out var emailIdValue))
        {
            originalEmailId = emailIdValue;
            toEmailAddress = $"{splitEmail[0]}@{splitEmail[2]}";
        }

        // Max ~20mb
        var tooBig = emailStream.Length > 20_000_000;
        if (!tooBig || originalEmailId.HasValue)
        {
            await _emailContentService.SaveInboundEmail(emailId, emailStream);
            emailStream.Position = 0;
        }

        if (originalEmailId.HasValue)
        {
            var originalEmail = await _db.Emails.Where(e => e.Id == originalEmailId && e.ToEmailAddress == toEmailAddress).Include(e => e.Reachable).FirstOrDefaultAsync();
            if (originalEmail != null)
            {
                incomingEmail.OriginalEmailId = originalEmail.Id;
                incomingEmail.ReachableId = originalEmail.ReachableId;
                incomingEmail.Type = EmailType.OutboundReply;
                await _db.SaveChangesAsync();

                var body = parsedEmail.Html;
                var split = body.Split("~--~");
                if (split.Length == 5)
                    body = split[0] + split[2] + split[4];

                var newValues = new
                {
                    Subject = "RE: " + originalEmail.Subject.Truncate(Constants.MaxSubjectLength - 4),
                    ToEmailAddress = originalEmail.FromEmailAddress,
                    ToEmailName = originalEmail.FromEmailName,
                    FromEmailAddress = originalEmail.ToEmailAddress,
                    FromEmailName = originalEmail.ToEmailName,
                    Body = body,
                };

                var forwardedEmailId = Guid.NewGuid();
                var forwardedEmail = new DbEmail()
                {
                    Id = forwardedEmailId,
                    Type = EmailType.OutboundForward,
                    ReachableId = originalEmail.ReachableId,
                    OriginalEmailId = incomingEmail.Id,
                    ToEmailAddress = newValues.ToEmailAddress,
                    ToEmailName = newValues.ToEmailName,
                    FromEmailAddress = newValues.FromEmailAddress,
                    FromEmailName = newValues.FromEmailName,
                    Subject = newValues.Subject,
                };
                _db.Emails.Add(forwardedEmail);
                await _db.SaveChangesAsync();
                await _emailContentService.SaveOutboundEmail(forwardedEmailId, newValues.Body);

                await _sendGridFacade.SendEmail(newValues.ToEmailAddress, newValues.ToEmailName, newValues.FromEmailAddress, newValues.FromEmailName, newValues.Subject, newValues.Body, emailAttachments: parsedEmail.GetAttachments());
                forwardedEmail.SentDate = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }
        else
        {
            var reachable = _db.Reachables.FirstOrDefault(reach => reach.ReacherEmailAddress == incomingEmail.ToEmailAddress);
            if (reachable == null)
            {
                incomingEmail.Type = EmailType.Failed;
                await _db.SaveChangesAsync();
                return;
            }
            incomingEmail.Reachable = reachable;
            if (reachable.Disabled)
            {
                incomingEmail.Type = EmailType.Disabled;
                await _db.SaveChangesAsync();
            }
            else if (tooBig)
            {
                incomingEmail.Type = EmailType.TooBig;
                await _db.SaveChangesAsync();
                return;
            }
            else
            {
                var mostRecentInboundFrom = _db.Emails.Where(e => e.ReachableId == reachable.Id && e.Type == EmailType.InboundReach && e.FromEmailAddress == incomingEmail.FromEmailAddress).OrderByDescending(e => e.CreatedDate).FirstOrDefault();
                if (mostRecentInboundFrom?.CreatedDate > DateTime.UtcNow.AddMinutes(-2))
                {
                    incomingEmail.Type = EmailType.TooSoon;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var whitelist = await _db.Whitelist.Where(e => e.UserId == reachable.UserId).Select(e => e.EmailAddress.ToUpper()).ToListAsync();
                    var isInWhitelist = whitelist.Any(w => incomingEmail.FromEmailAddress.ToUpper().EndsWith(w.ToUpper()));
                    if (isInWhitelist)
                    {
                        await _emailForwardingService.ForwardEmail(incomingEmail.Id);
                        return;
                    }

                    incomingEmail.Type = EmailType.InboundReach;
                    incomingEmail.InvoiceStatus = InvoiceStatus.Requested;
                    incomingEmail.CostUsd = reachable.CostUsdToReach;
                    await _db.SaveChangesAsync();
                    incomingEmail.StrikeInvoiceId = await _strikeFacade.CreateInvoice(reachable.StrikeUsername, reachable.CostUsdToReach, reachable.Currency, $"Email from {incomingEmail.FromEmailAddress}", incomingEmail.Id);
                    await _db.SaveChangesAsync();

                    var body = await _emailContentRenderer.GetPayEmailBody(reachable.ReacherEmailAddress, reachable.Name, incomingEmail.Id, incomingEmail.Subject ?? "(no subject)", incomingEmail.CostUsd ?? 0, parsedEmail.Html, parsedEmail.Attachments?.Length ?? 0);

                    var newValues = new
                    {
                        Subject = $"Your message to {reachable.Name}",
                        ToEmailAddress = incomingEmail.FromEmailAddress,
                        ToEmailName = incomingEmail.FromEmailName,
                        FromEmailAddress = reachable.ReacherEmailAddress,
                        FromEmailName = "Reacher",
                        Body = body,
                    };

                    var forwardedEmailId = Guid.NewGuid();
                    var forwardedEmail = new DbEmail()
                    {
                        Id = forwardedEmailId,
                        Type = EmailType.PaymentRequest,
                        Reachable = reachable,
                        OriginalEmailId = incomingEmail.Id,
                        ToEmailAddress = newValues.ToEmailAddress,
                        ToEmailName = newValues.ToEmailName,
                        FromEmailAddress = newValues.FromEmailAddress,
                        FromEmailName = newValues.FromEmailName,
                        Subject = newValues.Subject,
                    };
                    _db.Emails.Add(forwardedEmail);
                    await _db.SaveChangesAsync();
                    await _emailContentService.SaveOutboundEmail(forwardedEmailId, newValues.Body);

                    await _sendGridFacade.SendEmail(newValues.ToEmailAddress, newValues.ToEmailName, newValues.FromEmailAddress, newValues.FromEmailName, newValues.Subject, newValues.Body);

                    forwardedEmail.SentDate = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
            }
        }
    }
}

public interface IEmailIngestionService
{
    Task IngestEmail(Stream emailMessage);
}