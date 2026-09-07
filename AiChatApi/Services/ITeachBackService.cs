using AiChatApi.Models;

namespace AiChatApi.Services;

public interface ITeachBackService
{
    Task<TeachBackResponse> EvaluateExplanationAsync(string topic, string explanation);
}