using Microsoft.AspNetCore.Mvc;

namespace Reacher.App.Controllers;
[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly ILogger<InvoiceController> _logger;
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(ILogger<InvoiceController> logger, IInvoiceService invoiceService)
    {
        _logger = logger;
        _invoiceService = invoiceService;
    }

    [HttpGet("{id}")]
    public Task<Invoice> Get(Guid id)
    {
        return _invoiceService.GetInvoice(id);
    }
    [HttpPost("{id}/lninvoice")]
    public Task<LightningInvoice> LnInvoice(Guid id)
    {
        return _invoiceService.CreateLnInvoice(id);
    }
    [HttpGet("{id}/paid")]
    public async Task<InvoiceStatusResponse> Paid(Guid id)
    {
        var response = await _invoiceService.InvoiceIsPaid(id);
        return new() { Paid = response };
    }
}
