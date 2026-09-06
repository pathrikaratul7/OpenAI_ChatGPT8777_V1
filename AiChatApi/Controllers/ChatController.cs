using AiChatApi.Models;
using AiChatApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiChatApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IAiService _aiService;

    public ChatController(IAiService aiService)
    {
        _aiService = aiService;
    }

    // POST api/chat
    [HttpPost]
    public async Task<ActionResult<ChatResponseDto>> PostMessage([FromBody] ChatRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        try 
        {
            var aiReply = await _aiService.GetAiResponseAsync(request.Message);
            return Ok(new ChatResponseDto { Reply = aiReply });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Something went wrong: {ex.Message}");
        }
    }
}
