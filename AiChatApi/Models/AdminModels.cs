namespace AiChatApi.Models;

/// <summary>
/// Request model to update user tokens (Admin only)
/// </summary>
public class UpdateUserTokensRequest
{
    public int UserId { get; set; }
    public long AvailableTokens { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Response model for token update
/// </summary>
public class UpdateUserTokensResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public long AvailableTokens { get; set; }
    public long UsedTokens { get; set; }
    public long RemainingTokens { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Request model to reset user tokens (Admin only)
/// </summary>
public class ResetUserTokensRequest
{
    public int UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Response model for user list (Admin only)
/// </summary>
public class UserListItemResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public long AvailableTokens { get; set; }
    public long UsedTokens { get; set; }
    public long RemainingTokens { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request model to assign role to user (SuperAdmin only)
/// </summary>
public class AssignRoleRequest
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty; // "User", "Admin", "SuperAdmin"
}

/// <summary>
/// Response model for role assignment
/// </summary>
public class AssignRoleResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string NewRole { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}