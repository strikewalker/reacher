using Microsoft.AspNetCore.Mvc;

namespace Reacher.App.Controllers;
[ApiController]
[Route("api/[controller]")]
public class StrikeInboundController : ControllerBase
{
    private readonly ILogger<StrikeInboundController> _logger;
    private readonly IInvoiceService _invoiceService;

    public StrikeInboundController(ILogger<StrikeInboundController> logger, IInvoiceService invoiceService)
    {
        _logger = logger;
        _invoiceService = invoiceService;
    }

    [HttpPost]
    public Task Post([FromBody] InvoiceWebhookEvent body)
    {
        return _invoiceService.HandlePaidInvoice(body.Data.EntityId);
    }
}
public class InvoiceWebhookEvent
{
    public InvoiceRef Data { get; set; }
}
public class InvoiceRef
{
    public Guid EntityId { get; set; }
}