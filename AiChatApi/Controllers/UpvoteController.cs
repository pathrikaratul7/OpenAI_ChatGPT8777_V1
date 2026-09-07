using AiChatApi.Models;
using AiChatApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiChatApi.Controllers;

[ApiController]
[Route("api/upvotes")]
public class UpvoteController : ControllerBase
{
    private readonly IUpvoteService _upvoteService;

    public UpvoteController(IUpvoteService upvoteService)
    {
        _upvoteService = upvoteService;
    }

    /// <summary>
    /// POST /api/upvotes - Upvote a post
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UpvoteResponse>> UpvotePost([FromBody] UpvoteRequest request)
    {
        if (request.PostId <= 0)
        {
            return BadRequest("Valid PostId is required.");
        }

        try
        {
            var result = await _upvoteService.UpvotePostAsync(request.PostId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error upvoting post: {ex.Message}");
        }
    }
}