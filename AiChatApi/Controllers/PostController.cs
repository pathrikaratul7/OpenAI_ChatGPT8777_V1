using AiChatApi.Models;
using AiChatApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiChatApi.Controllers;

[Authorize]
[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// POST /api/posts - Create a new post (requires JWT token)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PostDoubtResponse>> CreatePost([FromBody] PostDoubtRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _postService.CreatePostAsync(request);
            return CreatedAtAction(nameof(GetPostById), new { id = response.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    /// <summary>
    /// GET /api/posts - Get all posts (no authentication required)
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDoubtResponse>>> GetAllPosts()
    {
        try
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving posts: {ex.Message}");
        }
    }

    /// <summary>
    /// GET /api/posts/{id} - Get post by ID (no authentication required)
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<PostDoubtResponse>> GetPostById(int id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving post: {ex.Message}");
        }
    }

    /// <summary>
    /// DELETE /api/posts/{id} - Delete a post (requires JWT token and ownership)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            var result = await _postService.DeletePostAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting post: {ex.Message}");
        }
    }
}