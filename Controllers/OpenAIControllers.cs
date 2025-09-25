using Microsoft.AspNetCore.Mvc;
using bot_messenger.Services;

namespace bot_messenger.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpenAIController : ControllerBase
{
    private readonly OpenAIService _openAIService;

    public OpenAIController(OpenAIService openAIService)
    {
        _openAIService = openAIService;
    }

    [HttpPost("generate-text")]
    public async Task<IActionResult> GenerateText(string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
        {
            return BadRequest(new { error = "Prompt cannot be empty." });
        }
        var result = await _openAIService.CompleteChat(prompt);
        return Ok(new { response = result });
    }
}
