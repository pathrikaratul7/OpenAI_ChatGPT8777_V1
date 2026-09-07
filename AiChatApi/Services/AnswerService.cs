using AiChatApi.Data;
using AiChatApi.Data.Entities;
using AiChatApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AiChatApi.Services;

public class AnswerService : IAnswerService
{
    private readonly TeachWallDbContext _context;

    public AnswerService(TeachWallDbContext context)
    {
        _context = context;
    }

    public async Task<PostAnswerResponse> CreateAnswerAsync(PostAnswerRequest request)
    {
        var answer = new Answer
        {
            PostId = request.PostId,
            AnswerText = request.Answer,
            Author = request.Author,
            CreatedAt = DateTime.UtcNow
        };

        _context.Answers.Add(answer);
        await _context.SaveChangesAsync();

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
        var answer = await _context.Answers.FindAsync(id);
        if (answer == null)
        {
            return false;
        }

        _context.Answers.Remove(answer);
        await _context.SaveChangesAsync();
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