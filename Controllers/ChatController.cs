namespace bot_messenger.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskQuestion([FromBody] ChatRequest request)
    {
        try
        {
            var response = await _chatService.GetContextAwareResponseAsync(request.Question);

            return Ok(new { response });

        }
        catch (Exception ex)
        {
            return StatusCode(500, new { err = ex.Message });
        }
    }
}

public class ChatRequest
{
    public string Question { get; set; }
}
