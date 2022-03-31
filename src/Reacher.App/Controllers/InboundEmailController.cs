using Microsoft.AspNetCore.Mvc;

namespace Reacher.App.Controllers;
[ApiController]
[Route("api/[controller]")]
public class InboundEmailController : ControllerBase
{
    private readonly ILogger<InboundEmailController> _logger;
    private readonly IEmailIngestionService _emailIngestionService;

    public InboundEmailController(ILogger<InboundEmailController> logger, IEmailIngestionService emailIngestionService)
    {
        _logger = logger;
        _emailIngestionService = emailIngestionService;
    }

    [HttpPost]
    public async Task Post() {
        await _emailIngestionService.IngestEmail(Request.Body);
    }
}
