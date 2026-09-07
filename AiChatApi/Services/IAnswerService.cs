using AiChatApi.Models;

namespace AiChatApi.Services;

public interface IAnswerService
{
    Task<PostAnswerResponse> CreateAnswerAsync(PostAnswerRequest request);
    Task<IEnumerable<PostAnswerResponse>> GetAnswersByPostIdAsync(int postId);
    Task<PostAnswerResponse?> GetAnswerByIdAsync(int id);
    Task<bool> DeleteAnswerAsync(int id);
}