using AiChatApi.Models;

namespace AiChatApi.Services;

public interface IPostService
{
    Task<PostDoubtResponse> CreatePostAsync(PostDoubtRequest request);
    Task<IEnumerable<PostDoubtResponse>> GetAllPostsAsync();
    Task<PostDoubtResponse?> GetPostByIdAsync(int id);
    Task<bool> DeletePostAsync(int id);
}