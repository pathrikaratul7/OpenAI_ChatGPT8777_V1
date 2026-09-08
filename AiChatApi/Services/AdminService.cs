using AiChatApi.Constants;
using AiChatApi.Data;
using AiChatApi.Data.Entities;
using AiChatApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AiChatApi.Services;

public class AdminService : IAdminService
{
    private readonly TeachWallDbContext _context;

    public AdminService(TeachWallDbContext context)
    {
        _context = context;
    }

    private async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    private void ValidateAdminAccess(User admin)
    {
        if (admin.Role != UserRole.Admin && admin.Role != UserRole.SuperAdmin)
        {
            throw new UnauthorizedAccessException("Only Admin or SuperAdmin can perform this action.");
        }
    }

    private void ValidateSuperAdminAccess(User superAdmin)
    {
        if (superAdmin.Role != UserRole.SuperAdmin)
        {
            throw new UnauthorizedAccessException("Only SuperAdmin can perform this action.");
        }
    }

    public async Task<UpdateUserTokensResponse> UpdateUserTokensAsync(int adminId, UpdateUserTokensRequest request)
    {
        // Validate admin
        var admin = await GetUserByIdAsync(adminId);
        if (admin == null)
        {
            throw new InvalidOperationException("Admin user not found.");
        }

        ValidateAdminAccess(admin);

        // Validate target user
        var targetUser = await GetUserByIdAsync(request.UserId);
        if (targetUser == null)
        {
            throw new InvalidOperationException($"User with ID {request.UserId} not found.");
        }

        if (request.AvailableTokens < 0)
        {
            throw new ArgumentException("Available tokens cannot be negative.");
        }

        // Calculate old tokens for logging
        var oldAvailableTokens = targetUser.AvailableTokens;

        // Update tokens
        targetUser.AvailableTokens = request.AvailableTokens;
        targetUser.UsedTokens = 0; // Reset used tokens when admin updates available

        // Log the action
        var tokenLog = new TokenUsageLog
        {
            UserId = request.UserId,
            TokensUsed = request.AvailableTokens - oldAvailableTokens,
            Action = "AdminUpdate",
            Description = $"Admin {admin.Username} updated tokens from {oldAvailableTokens} to {request.AvailableTokens}. Reason: {request.Reason}"
        };

        _context.TokenUsageLogs.Add(tokenLog);
        await _context.SaveChangesAsync();

        var remainingTokens = targetUser.AvailableTokens - targetUser.UsedTokens;

        return new UpdateUserTokensResponse
        {
            UserId = targetUser.Id,
            Username = targetUser.Username,
            Email = targetUser.Email,
            AvailableTokens = targetUser.AvailableTokens,
            UsedTokens = targetUser.UsedTokens,
            RemainingTokens = remainingTokens,
            Message = $"Tokens updated successfully. New balance: {remainingTokens}"
        };
    }

    public async Task<UpdateUserTokensResponse> ResetUserTokensAsync(int adminId, ResetUserTokensRequest request)
    {
        // Validate admin
        var admin = await GetUserByIdAsync(adminId);
        if (admin == null)
        {
            throw new InvalidOperationException("Admin user not found.");
        }

        ValidateAdminAccess(admin);

        // Validate target user
        var targetUser = await GetUserByIdAsync(request.UserId);
        if (targetUser == null)
        {
            throw new InvalidOperationException($"User with ID {request.UserId} not found.");
        }

        // Reset tokens to default
        const long defaultTokens = 25000L;
        var oldUsedTokens = targetUser.UsedTokens;

        targetUser.AvailableTokens = defaultTokens;
        targetUser.UsedTokens = 0;

        // Log the action
        var tokenLog = new TokenUsageLog
        {
            UserId = request.UserId,
            TokensUsed = -oldUsedTokens,
            Action = "TokenReset",
            Description = $"Admin {admin.Username} reset tokens to default ({defaultTokens}). Previous used tokens: {oldUsedTokens}. Reason: {request.Reason}"
        };

        _context.TokenUsageLogs.Add(tokenLog);
        await _context.SaveChangesAsync();

        return new UpdateUserTokensResponse
        {
            UserId = targetUser.Id,
            Username = targetUser.Username,
            Email = targetUser.Email,
            AvailableTokens = targetUser.AvailableTokens,
            UsedTokens = targetUser.UsedTokens,
            RemainingTokens = defaultTokens,
            Message = "Tokens reset to default successfully."
        };
    }

    public async Task<IEnumerable<UserListItemResponse>> GetAllUsersAsync(int adminId)
    {
        // Validate admin
        var admin = await GetUserByIdAsync(adminId);
        if (admin == null)
        {
            throw new InvalidOperationException("Admin user not found.");
        }

        ValidateAdminAccess(admin);

        var users = await _context.Users
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();

        return users.Select(u => new UserListItemResponse
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Role = u.Role.ToString(),
            AvailableTokens = u.AvailableTokens,
            UsedTokens = u.UsedTokens,
            RemainingTokens = u.AvailableTokens - u.UsedTokens,
            CreatedAt = u.CreatedAt
        });
    }

    public async Task<UserListItemResponse> GetUserByIdAsync(int adminId, int userId)
    {
        // Validate admin
        var admin = await GetUserByIdAsync(adminId);
        if (admin == null)
        {
            throw new InvalidOperationException("Admin user not found.");
        }

        ValidateAdminAccess(admin);

        var user = await GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found.");
        }

        return new UserListItemResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            AvailableTokens = user.AvailableTokens,
            UsedTokens = user.UsedTokens,
            RemainingTokens = user.AvailableTokens - user.UsedTokens,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<AssignRoleResponse> AssignRoleToUserAsync(int superAdminId, AssignRoleRequest request)
    {
        // Validate SuperAdmin
        var superAdmin = await GetUserByIdAsync(superAdminId);
        if (superAdmin == null)
        {
            throw new InvalidOperationException("SuperAdmin user not found.");
        }

        ValidateSuperAdminAccess(superAdmin);

        // Validate target user
        var targetUser = await GetUserByIdAsync(request.UserId);
        if (targetUser == null)
        {
            throw new InvalidOperationException($"User with ID {request.UserId} not found.");
        }

        // Parse the role
        if (!Enum.TryParse<UserRole>(request.Role, true, out var newRole))
        {
            throw new ArgumentException($"Invalid role. Valid roles are: User, Admin, SuperAdmin");
        }

        var oldRole = targetUser.Role;
        targetUser.Role = newRole;

        await _context.SaveChangesAsync();

        return new AssignRoleResponse
        {
            UserId = targetUser.Id,
            Username = targetUser.Username,
            NewRole = newRole.ToString(),
            Message = $"Role updated from {oldRole} to {newRole} successfully."
        };
    }
}