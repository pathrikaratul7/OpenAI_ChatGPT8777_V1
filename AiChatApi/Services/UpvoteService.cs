using AiChatApi.Data;
using AiChatApi.Data.Entities;
using AiChatApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AiChatApi.Services;

public class UpvoteService : IUpvoteService
{
    private readonly TeachWallDbContext _context;

    public UpvoteService(TeachWallDbContext context)
    {
        _context = context;
    }

    public async Task<UpvoteResponse> UpvotePostAsync(int postId)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
        {
            throw new InvalidOperationException($"Post with ID {postId} not found.");
        }

        // Increment upvote count
        post.Upvotes++;
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();

        return new UpvoteResponse
        {
            PostId = postId,
            TotalUpvotes = post.Upvotes
        };
    }
}