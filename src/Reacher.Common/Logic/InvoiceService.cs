using Reacher.Common.Models;
using StrongGrid.Models.Webhooks;

namespace Reacher.Common.Logic;

public class InvoiceService : IInvoiceService
{
    private readonly IStrikeFacade _strikeFacade;
    private readonly ISendGridFacade _sendGridFacade;
    private readonly AppDbContext _db;
    private readonly IEmailContentRenderer _emailContentRenderer;
    private readonly IEmailContentService _emailContentService;
    private readonly ISendGridParser _sendGridParser;

    public InvoiceService(IStrikeFacade strikeFacade, ISendGridFacade sendGridFacade, AppDbContext db, IEmailContentRenderer emailContentRenderer,
        IEmailContentService emailContentService, ISendGridParser sendGridParser)
    {
        _strikeFacade = strikeFacade;
        _sendGridFacade = sendGridFacade;
        _db = db;
        _emailContentRenderer = emailContentRenderer;
        _emailContentService = emailContentService;
        _sendGridParser = sendGridParser;
    }

    public async Task<Invoice> GetInvoice(Guid emailId)
    {
        var inv = await _db.Emails.Where(e => e.Id == emailId).Select(e => new Invoice
        {
            Id = e.Id,
            CostUsd = e.CostUsd.Value,
            StrikeInvoiceId = e.StrikeInvoiceId.Value,
            Paid = e.InvoiceStatus == InvoiceStatus.Paid,
            Reachable = new()
            {
                EmailAddress = e.Reachable.ReacherEmailAddress,
                Id = e.Reachable.Id,
                Name = e.Reachable.Name
            }
        }).SingleAsync();
        if (!inv.Paid)
        {
            inv.Paid = await _strikeFacade.InvoiceIsPaid(inv.StrikeInvoiceId);
            if (inv.Paid)
            {
                await HandlePaidInvoice(inv.StrikeInvoiceId);
            }
        }
        return inv;
    }

    public async Task HandlePaidInvoice(Guid strikeInvoiceId)
    {
        var isPaid = await _strikeFacade.InvoiceIsPaid(strikeInvoiceId);
        if (!isPaid)
        {
            return;
        }

        var inboundEmail = await _db.Emails.Where(e => e.StrikeInvoiceId == strikeInvoiceId).Include(e => e.Reachable).FirstAsync();
        if (inboundEmail.InvoiceStatus != InvoiceStatus.Paid)
        {
            inboundEmail.InvoiceStatus = InvoiceStatus.Paid;
            inboundEmail.PaidDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            var originalEmailStream = await _emailContentService.GetInboundEmail(inboundEmail.Id);
            var parsed = await _sendGridParser.ParseSendGridInboundEmail(originalEmailStream);
            await HandleForwardedEmail(inboundEmail, parsed);
            await HandlePaymentSuccessEmail(inboundEmail, parsed);
        }
    }

    private async Task HandlePaymentSuccessEmail(DbEmail inboundEmail, InboundEmail parsedEmail)
    {
        var reachable = inboundEmail.Reachable;

        var newSubject = $"Your message to {reachable.Name} was delivered successfully";

        var newValues = new
        {
            Subject = newSubject,
            ToEmailAddress = inboundEmail.FromEmailAddress,
            ToEmailName = inboundEmail.FromEmailName,
            FromEmailAddress = "noreply@reacher.me",
            FromEmailName = "Reacher",
            Body = await _emailContentRenderer.GetPaymentSuccessEmailBody(reachable.ReacherEmailAddress, reachable.Name, inboundEmail.Subject ?? "(no subject)", inboundEmail.CostUsd ?? 0, parsedEmail.Html, parsedEmail.Attachments?.Length ?? 0)
        };

        var paySuccessEmailId = Guid.NewGuid();
        var paySuccessEmail = new DbEmail()
        {
            Id = paySuccessEmailId,
            Type = EmailType.PaymentSuccess,
            Reachable = reachable,
            OriginalEmailId = inboundEmail.Id,
            ToEmailAddress = newValues.ToEmailAddress,
            ToEmailName = newValues.ToEmailName,
            FromEmailAddress = newValues.FromEmailAddress,
            FromEmailName = newValues.FromEmailName,
            Subject = newValues.Subject,
        };
        _db.Emails.Add(paySuccessEmail);
        await _db.SaveChangesAsync();

        await _emailContentService.SaveOutboundEmail(paySuccessEmailId, newValues.Body);

        await _sendGridFacade.SendEmail(newValues.ToEmailAddress, newValues.ToEmailName, newValues.FromEmailAddress, newValues.FromEmailName, newValues.Subject, newValues.Body);
        paySuccessEmail.SentDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
    private async Task HandleForwardedEmail(DbEmail inboundEmail, InboundEmail parsedEmail)
    {
        var reachable = inboundEmail.Reachable;

        var newSubject = $"New Reacher Email";
        newSubject = ($"{newSubject}: {inboundEmail.Subject}").Truncate(Constants.MaxSubjectLength);

        var splitEmail = reachable.ReacherEmailAddress.Split('@');

        var reacherEmailAddress = $"{splitEmail[0]}+{inboundEmail.Id}@{splitEmail[1]}";

        var newValues = new
        {
            Subject = newSubject,
            ToEmailAddress = reachable.ToEmailAddress,
            ToEmailName = reachable.Name,
            FromEmailAddress = reachable.ReacherEmailAddress,
            FromEmailName = "Reacher",
            ReplyToEmailAddress = reacherEmailAddress,
            Body = await _emailContentRenderer.GetFowardEmail(parsedEmail.Html, inboundEmail.FromEmailAddress, inboundEmail.FromEmailName, inboundEmail.Subject, inboundEmail.CostUsd ?? 0, inboundEmail.Reachable!.ReacherEmailAddress)
        };

        var forwardedEmailId = Guid.NewGuid();
        var forwardedEmail = new DbEmail()
        {
            Id = forwardedEmailId,
            Type = EmailType.InboundForward,
            Reachable = reachable,
            OriginalEmailId = inboundEmail.Id,
            ToEmailAddress = newValues.ToEmailAddress,
            ToEmailName = newValues.ToEmailName,
            FromEmailAddress = newValues.FromEmailAddress,
            FromEmailName = newValues.FromEmailName,
            Subject = newValues.Subject,
        };
        _db.Emails.Add(forwardedEmail);
        await _db.SaveChangesAsync();

        await _emailContentService.SaveOutboundEmail(forwardedEmailId, newValues.Body);

        await _sendGridFacade.SendEmail(newValues.ToEmailAddress, newValues.ToEmailName, newValues.FromEmailAddress, newValues.FromEmailName, newValues.Subject, newValues.Body, newValues.ReplyToEmailAddress, parsedEmail.GetAttachments());
        forwardedEmail.SentDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> InvoiceIsPaid(Guid strikeInvoiceId)
    {
        var result = await _db.Emails.Where(e => e.StrikeInvoiceId == strikeInvoiceId).Select(e => e.InvoiceStatus).SingleOrDefaultAsync();
        return result == InvoiceStatus.Paid;
    }

    public async Task<LightningInvoice> CreateLnInvoice(Guid strikeInvoiceId)
    {
        var temp = await _strikeFacade.CreateLnInvoice(strikeInvoiceId);
        return new() { ExpirationInSeconds = temp.ExpirationInSec, LnInvoiceId = temp.LnInvoice };
    }
}

public interface IInvoiceService
{
    Task<LightningInvoice> CreateLnInvoice(Guid strikeInvoiceId);
    Task<Invoice> GetInvoice(Guid emailId);
    Task HandlePaidInvoice(Guid strikeInvoiceId);
    Task<bool> InvoiceIsPaid(Guid strikeInvoiceId);
}
