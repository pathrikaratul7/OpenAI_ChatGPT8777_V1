namespace AiChatApi.Services;

public interface IAiService
{
    Task<string> GetAiResponseAsync(string userMessage);
}
