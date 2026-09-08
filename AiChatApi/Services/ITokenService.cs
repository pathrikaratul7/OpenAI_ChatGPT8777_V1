namespace AiChatApi.Services;

public interface ITokenService
{
    Task<(bool Success, string Message, long RemainingTokens)> DeductTokensAsync(int userId, string content, string action);
    Task<(bool CanProceed, string Message, long RemainingTokens)> CheckTokenAvailabilityAsync(int userId, string content);
    Task<long> GetUserRemainingTokensAsync(int userId);
    Task<bool> RefundTokensAsync(int userId, long tokens, string reason);
}