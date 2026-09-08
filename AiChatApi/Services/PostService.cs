using AiChatApi.Data;
using AiChatApi.Data.Entities;
using AiChatApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AiChatApi.Services;

public class PostService : IPostService
{
    private readonly TeachWallDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    public PostService(TeachWallDbContext context, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token.");
        }
        return userId;
    }

    public async Task<PostDoubtResponse> CreatePostAsync(PostDoubtRequest request)
    {
        var userId = GetCurrentUserId();

        // Validate user exists
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Title) || 
            string.IsNullOrWhiteSpace(request.Description))
        {
            throw new ArgumentException("Title and description are required.");
        }

        // Combine title and description to calculate tokens
        var combinedContent = $"{request.Title} {request.Description}";

        // Check token availability before creating post
        var tokenCheck = await _tokenService.CheckTokenAvailabilityAsync(userId, combinedContent);
        if (!tokenCheck.CanProceed)
        {
            throw new InvalidOperationException(tokenCheck.Message);
        }

        var post = new Post
        {
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            Subject = request.Subject,
            Author = request.Author,
            Upvotes = 0
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        // Deduct tokens after successful post creation
        await _tokenService.DeductTokensAsync(userId, combinedContent, "PostCreated");

        return MapToResponse(post);
    }

    public async Task<IEnumerable<PostDoubtResponse>> GetAllPostsAsync()
    {
        var posts = await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return posts.Select(MapToResponse);
    }

    public async Task<PostDoubtResponse?> GetPostByIdAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        return post == null ? null : MapToResponse(post);
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        var userId = GetCurrentUserId();
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
        {
            return false;
        }

        // Only allow post owner to delete
        if (post.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own posts.");
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        // Refund tokens when post is deleted
        var combinedContent = $"{post.Title} {post.Description}";
        var tokensToRefund = combinedContent.Length;
        await _tokenService.RefundTokensAsync(userId, tokensToRefund, "PostDeleted");

        return true;
    }

    private static PostDoubtResponse MapToResponse(Post post)
    {
        return new PostDoubtResponse
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            Subject = post.Subject,
            Author = post.Author,
            Upvotes = post.Upvotes,
            CreatedAt = post.CreatedAt
        };
    }
}