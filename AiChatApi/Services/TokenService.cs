using AiChatApi.Data;
using AiChatApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiChatApi.Services;

public class TokenService : ITokenService
{
    private readonly TeachWallDbContext _context;

    public TokenService(TeachWallDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message, long RemainingTokens)> DeductTokensAsync(
        int userId, string content, string action)
    {
        var tokensRequired = CalculateTokensRequired(content);
        var canDeduct = await CheckTokenAvailabilityAsync(userId, content);

        if (!canDeduct.CanProceed)
        {
            return (false, canDeduct.Message, canDeduct.RemainingTokens);
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return (false, "User not found.", 0);
        }

        user.UsedTokens += tokensRequired;

        var tokenLog = new TokenUsageLog
        {
            UserId = userId,
            TokensUsed = tokensRequired,
            Action = action,
            Description = $"Deducted {tokensRequired} tokens for {action}"
        };

        _context.TokenUsageLogs.Add(tokenLog);
        await _context.SaveChangesAsync();

        var remainingTokens = user.AvailableTokens - user.UsedTokens;
        return (true, $"Successfully deducted {tokensRequired} tokens.", remainingTokens);
    }

    public async Task<(bool CanProceed, string Message, long RemainingTokens)> CheckTokenAvailabilityAsync(
        int userId, string content)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return (false, "User not found.", 0);
        }

        var tokensRequired = CalculateTokensRequired(content);
        var remainingTokens = user.AvailableTokens - user.UsedTokens;

        if (remainingTokens <= 0)
        {
            return (false, "You have exhausted your token limit. Please upgrade your subscription.", 0);
        }

        if (remainingTokens < tokensRequired)
        {
            return (false, $"Insufficient tokens. Required: {tokensRequired}, Available: {remainingTokens}.", remainingTokens);
        }

        return (true, "Sufficient tokens available.", remainingTokens);
    }

    public async Task<long> GetUserRemainingTokensAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        return user.AvailableTokens - user.UsedTokens;
    }

    public async Task<bool> RefundTokensAsync(int userId, long tokens, string reason)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.UsedTokens = Math.Max(0, user.UsedTokens - tokens);

        var tokenLog = new TokenUsageLog
        {
            UserId = userId,
            TokensUsed = -tokens,
            Action = "Refund",
            Description = $"Refunded {tokens} tokens. Reason: {reason}"
        };

        _context.TokenUsageLogs.Add(tokenLog);
        await _context.SaveChangesAsync();

        return true;
    }

    private static long CalculateTokensRequired(string content)
    {
        // Simple calculation: 1 token per character
        // You can adjust this based on your token counting logic
        return content.Length;
    }
}