using AiChatApi.Data;
using AiChatApi.Data.Entities;
using AiChatApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AiChatApi.Services;

public class AnswerService : IAnswerService
{
    private readonly TeachWallDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    public AnswerService(TeachWallDbContext context, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
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

    public async Task<PostAnswerResponse> CreateAnswerAsync(PostAnswerRequest request)
    {
        var userId = GetCurrentUserId();

        // Validate that the Post exists
        var postExists = await _context.Posts.AnyAsync(p => p.Id == request.PostId);
        if (!postExists)
        {
            throw new InvalidOperationException($"Post with ID {request.PostId} does not exist.");
        }

        // Validate user exists
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        // Check token availability before creating answer
        var tokenCheck = await _tokenService.CheckTokenAvailabilityAsync(userId, request.Answer);
        if (!tokenCheck.CanProceed)
        {
            throw new InvalidOperationException(tokenCheck.Message);
        }

        var answer = new Answer
        {
            UserId = userId,
            PostId = request.PostId,
            AnswerText = request.Answer,
            Author = request.Author
        };

        _context.Answers.Add(answer);
        await _context.SaveChangesAsync();

        // Deduct tokens after successful answer creation
        await _tokenService.DeductTokensAsync(userId, request.Answer, "AnswerCreated");

        return MapToResponse(answer);
    }

    public async Task<IEnumerable<PostAnswerResponse>> GetAnswersByPostIdAsync(int postId)
    {
        var answers = await _context.Answers
            .Where(a => a.PostId == postId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();

        return answers.Select(MapToResponse);
    }

    public async Task<PostAnswerResponse?> GetAnswerByIdAsync(int id)
    {
        var answer = await _context.Answers.FindAsync(id);
        return answer == null ? null : MapToResponse(answer);
    }

    public async Task<bool> DeleteAnswerAsync(int id)
    {
        var userId = GetCurrentUserId();
        var answer = await _context.Answers.FindAsync(id);

        if (answer == null)
        {
            return false;
        }

        // Only allow answer owner to delete
        if (answer.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own answers.");
        }

        _context.Answers.Remove(answer);
        await _context.SaveChangesAsync();

        // Refund tokens when answer is deleted
        var tokensToRefund = answer.AnswerText.Length;
        await _tokenService.RefundTokensAsync(userId, tokensToRefund, "AnswerDeleted");

        return true;
    }

    private static PostAnswerResponse MapToResponse(Answer answer)
    {
        return new PostAnswerResponse
        {
            Id = answer.Id,
            PostId = answer.PostId,
            Answer = answer.AnswerText,
            Author = answer.Author,
            CreatedAt = answer.CreatedAt
        };
    }
}