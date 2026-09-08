namespace AiChatApi.Models;

/// <summary>
/// Response model for user profile/subscription info
/// </summary>
public class UserProfileResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public long AvailableTokens { get; set; }
    public long UsedTokens { get; set; }
    public long RemainingTokens { get; set; }
    public double UsagePercentage { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Response model for token usage log
/// </summary>
public class TokenUsageLogResponse
{
    public int Id { get; set; }
    public long TokensUsed { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Response model for subscription status
/// </summary>
public class SubscriptionStatusResponse
{
    public long RemainingTokens { get; set; }
    public long TotalTokens { get; set; }
    public long UsedTokens { get; set; }
    public double UsagePercentage { get; set; }
    public bool IsSubscriptionActive { get; set; }
}