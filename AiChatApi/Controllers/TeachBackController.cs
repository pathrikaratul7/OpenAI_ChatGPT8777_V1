using AiChatApi.Models;
using AiChatApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiChatApi.Controllers;

[ApiController]
[Route("api/teachback")]
public class TeachBackController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly ITeachBackService _teachBackService;

    public TeachBackController(IAiService aiService, ITeachBackService teachBackService)
    {
        _aiService = aiService;
        _teachBackService = teachBackService;
    }

    /// <summary>
    /// POST /api/teachback - Evaluate a user's explanation of a topic
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TeachBackResponse>> EvaluateExplanation([FromBody] TeachBackRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Topic) || string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Topic and text cannot be empty.");
        }

        try
        {
            var evaluation = await _teachBackService.EvaluateExplanationAsync(request.Topic, request.Text);
            return Ok(evaluation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error evaluating explanation: {ex.Message}");
        }
    }
}