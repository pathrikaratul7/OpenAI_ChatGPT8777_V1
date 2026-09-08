using AiChatApi.Models;

namespace AiChatApi.Services;

public interface ITeachBackService
{
    Task<TeachBackResponse> EvaluateExplanationAsync(int userId, string topic, string explanation);
}