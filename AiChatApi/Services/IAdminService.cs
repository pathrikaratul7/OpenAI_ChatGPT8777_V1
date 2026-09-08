using AiChatApi.Models;

namespace AiChatApi.Services;

public interface IAdminService
{
    Task<UpdateUserTokensResponse> UpdateUserTokensAsync(int adminId, UpdateUserTokensRequest request);
    Task<UpdateUserTokensResponse> ResetUserTokensAsync(int adminId, ResetUserTokensRequest request);
    Task<IEnumerable<UserListItemResponse>> GetAllUsersAsync(int adminId);
    Task<UserListItemResponse> GetUserByIdAsync(int adminId, int userId);
    Task<AssignRoleResponse> AssignRoleToUserAsync(int superAdminId, AssignRoleRequest request);
}