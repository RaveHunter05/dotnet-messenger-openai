using Microsoft.AspNetCore.Mvc;
using bot_messenger.Services;

namespace bot_messenger.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly WebhookService _webhookService;

    public WebhookController(WebhookService webhookService)
    {
        _webhookService = webhookService;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveWebhook([FromBody] dynamic payload)
    {
        if (payload == null)
        {
            return BadRequest(new { error = "Payload cannot be empty." });
        }
        await _webhookService.ProcessWebhook(payload);
        return Ok(new { status = "Webhook received and processed." });
    }
}
