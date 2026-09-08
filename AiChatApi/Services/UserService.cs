using AiChatApi.Data;
using AiChatApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AiChatApi.Services;

public class UserService : IUserService
{
    private readonly TeachWallDbContext _context;

    public UserService(TeachWallDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileResponse> GetUserProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var remainingTokens = user.AvailableTokens - user.UsedTokens;
        var usagePercentage = user.AvailableTokens > 0 
            ? (user.UsedTokens * 100.0) / user.AvailableTokens 
            : 0;

        return new UserProfileResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AvailableTokens = user.AvailableTokens,
            UsedTokens = user.UsedTokens,
            RemainingTokens = remainingTokens,
            UsagePercentage = usagePercentage,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<SubscriptionStatusResponse> GetSubscriptionStatusAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var remainingTokens = user.AvailableTokens - user.UsedTokens;
        var usagePercentage = user.AvailableTokens > 0 
            ? (user.UsedTokens * 100.0) / user.AvailableTokens 
            : 0;

        return new SubscriptionStatusResponse
        {
            RemainingTokens = remainingTokens,
            TotalTokens = user.AvailableTokens,
            UsedTokens = user.UsedTokens,
            UsagePercentage = usagePercentage,
            IsSubscriptionActive = remainingTokens > 0
        };
    }

    public async Task<IEnumerable<TokenUsageLogResponse>> GetTokenUsageHistoryAsync(int userId, int limit = 50)
    {
        var logs = await _context.TokenUsageLogs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return logs.Select(l => new TokenUsageLogResponse
        {
            Id = l.Id,
            TokensUsed = l.TokensUsed,
            Action = l.Action,
            Description = l.Description,
            CreatedAt = l.CreatedAt
        });
    }
}