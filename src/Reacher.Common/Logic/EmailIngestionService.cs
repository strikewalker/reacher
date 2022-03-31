namespace Reacher.Common.Logic;

public class EmailIngestionService : IEmailIngestionService
{
    private readonly AppDbContext _db;
    private readonly IEmailContentRenderer _emailContentRenderer;
    private readonly ISendGridParser _sendGridParser;
    private readonly IStrikeFacade _strikeFacade;
    private readonly ISendGridFacade _sendGridFacade;

    public EmailIngestionService(AppDbContext db, IEmailContentRenderer emailContentRenderer, ISendGridParser sendGridParser, IStrikeFacade strikeFacade, ISendGridFacade sendGridFacade)
    {
        _db = db;
        _emailContentRenderer = emailContentRenderer;
        _sendGridParser = sendGridParser;
        _strikeFacade = strikeFacade;
        _sendGridFacade = sendGridFacade;
    }

    public async Task IngestEmail(Stream emailMessage)
    {
        var incomingEmail = await _sendGridParser.ParseSendGridInboundEmail(emailMessage);
        incomingEmail.Id = Guid.NewGuid();
        _db.Emails.Add(incomingEmail);
        await _db.SaveChangesAsync();

        Guid? emailId = null;
        var toEmailAddress = incomingEmail.ToEmailAddress;
        var splitEmail = toEmailAddress?.Split('+', '@');
        if (splitEmail?.Length == 3 && Guid.TryParse(splitEmail[1], out var emailIdValue))
        {
            emailId = emailIdValue;
            toEmailAddress = $"{splitEmail[0]}@{splitEmail[2]}";
        }

        if (emailId.HasValue)
        {
            var originalEmail = await _db.Emails.Where(e => e.Id == emailId && e.ToEmailAddress == toEmailAddress).Include(e => e.Reachable).FirstOrDefaultAsync();
            if (originalEmail != null)
            {
                incomingEmail.OriginalEmailId = originalEmail.Id;
                incomingEmail.ReachableId = originalEmail.ReachableId;
                incomingEmail.Type = EmailType.OutboundReply;
                await _db.SaveChangesAsync();

                var body = incomingEmail.Body;
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

                var forwardedEmail = new DbEmail()
                {
                    Type = EmailType.OutboundForward,
                    ReachableId = originalEmail.ReachableId,
                    OriginalEmailId = incomingEmail.Id,
                    ToEmailAddress = newValues.ToEmailAddress,
                    ToEmailName = newValues.ToEmailName,
                    FromEmailAddress = newValues.FromEmailAddress,
                    FromEmailName = newValues.FromEmailName,
                    Subject = newValues.Subject,
                    Body = newValues.Body,
                };
                _db.Emails.Add(forwardedEmail);
                await _db.SaveChangesAsync();

                await _sendGridFacade.SendEmail(newValues.ToEmailAddress, newValues.ToEmailName, newValues.FromEmailAddress, newValues.FromEmailName, newValues.Subject, newValues.Body);
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
            }
            else
            {
                incomingEmail.Reachable = reachable;
                var mostRecentInboundFrom = _db.Emails.Where(e => e.ReachableId == reachable.Id && e.Type == EmailType.InboundReach && e.FromEmailAddress == incomingEmail.FromEmailAddress).OrderByDescending(e => e.CreatedDate).FirstOrDefault();
                if (mostRecentInboundFrom?.CreatedDate > DateTime.UtcNow.AddMinutes(-2))
                {
                    incomingEmail.Type = EmailType.TooSoon;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    incomingEmail.Type = EmailType.InboundReach;
                    incomingEmail.CostUsd = reachable.CostUsdToReach;
                    await _db.SaveChangesAsync();
                    incomingEmail.StrikeInvoiceId = await _strikeFacade.CreateInvoice(reachable.StrikeUsername, reachable.CostUsdToReach, $"Email from {incomingEmail.FromEmailAddress}", incomingEmail.Id);
                    await _db.SaveChangesAsync();

                    var body = await _emailContentRenderer.GetPayEmailBody(reachable.ReacherEmailAddress, reachable.Name, incomingEmail.Id, incomingEmail.Subject ?? "(no subject)", incomingEmail.CostUsd ?? 0, incomingEmail.Body);

                    var newValues = new
                    {
                        Subject = $"Your message to {reachable.Name}",
                        ToEmailAddress = incomingEmail.FromEmailAddress,
                        ToEmailName = incomingEmail.FromEmailName,
                        FromEmailAddress = reachable.ReacherEmailAddress,
                        FromEmailName = "Reacher",
                        Body = body,
                    };

                    var forwardedEmail = new DbEmail()
                    {
                        Type = EmailType.PaymentRequest,
                        Reachable = reachable,
                        OriginalEmailId = incomingEmail.Id,
                        ToEmailAddress = newValues.ToEmailAddress,
                        ToEmailName = newValues.ToEmailName,
                        FromEmailAddress = newValues.FromEmailAddress,
                        FromEmailName = newValues.FromEmailName,
                        Subject = newValues.Subject,
                        Body = newValues.Body,
                    };
                    _db.Emails.Add(forwardedEmail);
                    await _db.SaveChangesAsync();

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