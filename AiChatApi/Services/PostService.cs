using AiChatApi.Data;
using AiChatApi.Data.Entities;
using AiChatApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AiChatApi.Services;

public class PostService : IPostService
{
    private readonly TeachWallDbContext _context;

    public PostService(TeachWallDbContext context)
    {
        _context = context;
    }

    public async Task<PostDoubtResponse> CreatePostAsync(PostDoubtRequest request)
    {
        var post = new Post
        {
            Title = request.Title,
            Description = request.Description,
            Subject = request.Subject,
            Author = request.Author,
            Upvotes = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

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
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return false;
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
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