using AiChatApi.Models;
using AiChatApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiChatApi.Controllers;

[ApiController]
[Route("api/posts_old")]
public class PostsController_old : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController_old(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// POST /api/posts - Create a new doubt post
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PostDoubtResponse>> CreatePost([FromBody] PostDoubtRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Title and description cannot be empty.");
        }

        try
        {
            var post = await _postService.CreatePostAsync(request);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating post: {ex.Message}");
        }
    }

    /// <summary>
    /// GET /api/posts - Retrieve all posts
    /// </summary>
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
    /// GET /api/posts/{id} - Retrieve a specific post
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PostDoubtResponse>> GetPost(int id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            return Ok(post);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving post: {ex.Message}");
        }
    }

    /// <summary>
    /// DELETE /api/posts/{id} - Delete a post
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            var success = await _postService.DeletePostAsync(id);
            if (!success)
            {
                return NotFound("Post not found.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting post: {ex.Message}");
        }
    }
}