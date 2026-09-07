using AiChatApi.Models;
using AiChatApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiChatApi.Controllers;

[ApiController]
[Route("api/answers")]
public class AnswersController : ControllerBase
{
    private readonly IAnswerService _answerService;

    public AnswersController(IAnswerService answerService)
    {
        _answerService = answerService;
    }

    /// <summary>
    /// POST /api/answers - Post an answer to a doubt
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PostAnswerResponse>> CreateAnswer([FromBody] PostAnswerRequest request)
    {
        if (request.PostId <= 0 || string.IsNullOrWhiteSpace(request.Answer))
        {
            return BadRequest("Valid PostId and Answer are required.");
        }

        try
        {
            var answer = await _answerService.CreateAnswerAsync(request);
            return CreatedAtAction(nameof(GetAnswer), new { id = answer.Id }, answer);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating answer: {ex.Message}");
        }
    }

    /// <summary>
    /// GET /api/answers/{postId} - Retrieve all answers for a post
    /// </summary>
    [HttpGet("post/{postId}")]
    public async Task<ActionResult<IEnumerable<PostAnswerResponse>>> GetAnswersByPost(int postId)
    {
        try
        {
            var answers = await _answerService.GetAnswersByPostIdAsync(postId);
            return Ok(answers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving answers: {ex.Message}");
        }
    }

    /// <summary>
    /// GET /api/answers/{id} - Retrieve a specific answer
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PostAnswerResponse>> GetAnswer(int id)
    {
        try
        {
            var answer = await _answerService.GetAnswerByIdAsync(id);
            if (answer == null)
            {
                return NotFound("Answer not found.");
            }

            return Ok(answer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving answer: {ex.Message}");
        }
    }

    /// <summary>
    /// DELETE /api/answers/{id} - Delete an answer
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnswer(int id)
    {
        try
        {
            var success = await _answerService.DeleteAnswerAsync(id);
            if (!success)
            {
                return NotFound("Answer not found.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting answer: {ex.Message}");
        }
    }
}