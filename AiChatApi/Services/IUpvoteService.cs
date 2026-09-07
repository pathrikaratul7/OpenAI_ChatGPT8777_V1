using AiChatApi.Models;

namespace AiChatApi.Services;

public interface IUpvoteService
{
    Task<UpvoteResponse> UpvotePostAsync(int postId);
}