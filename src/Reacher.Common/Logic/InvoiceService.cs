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
    private readonly IEmailForwardingService _emailForwardingService;

    public InvoiceService(IStrikeFacade strikeFacade, ISendGridFacade sendGridFacade, AppDbContext db, IEmailContentRenderer emailContentRenderer,
        IEmailContentService emailContentService, ISendGridParser sendGridParser, IEmailForwardingService emailForwardingService)
    {
        _strikeFacade = strikeFacade;
        _sendGridFacade = sendGridFacade;
        _db = db;
        _emailContentRenderer = emailContentRenderer;
        _emailContentService = emailContentService;
        _sendGridParser = sendGridParser;
        _emailForwardingService = emailForwardingService;
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
            await _emailForwardingService.ForwardEmail(inboundEmail, true);
        }
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
