using AiChatApi.Models;

namespace AiChatApi.Services;

public interface IUserService
{
    Task<UserProfileResponse> GetUserProfileAsync(int userId);
    Task<SubscriptionStatusResponse> GetSubscriptionStatusAsync(int userId);
    Task<IEnumerable<TokenUsageLogResponse>> GetTokenUsageHistoryAsync(int userId, int limit = 50);
}