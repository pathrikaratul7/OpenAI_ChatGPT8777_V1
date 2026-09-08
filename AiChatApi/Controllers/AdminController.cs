using AiChatApi.Constants;
using AiChatApi.Models;
using AiChatApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AiChatApi.Controllers;

[Authorize]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token.");
        }
        return userId;
    }

    private string? GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value;
    }

    /// <summary>
    /// PUT /api/admin/users/{userId}/tokens - Update user tokens (Admin/SuperAdmin only)
    /// </summary>
    [HttpPut("users/{userId}/tokens")]
    public async Task<ActionResult<UpdateUserTokensResponse>> UpdateUserTokens(
        int userId, 
        [FromBody] UpdateUserTokensRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var adminId = GetCurrentUserId();
            request.UserId = userId;
            var response = await _adminService.UpdateUserTokensAsync(adminId, request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating tokens: {ex.Message}");
        }
    }

    /// <summary>
    /// POST /api/admin/users/{userId}/reset-tokens - Reset user tokens to default (Admin/SuperAdmin only)
    /// </summary>
    [HttpPost("users/{userId}/reset-tokens")]
    public async Task<ActionResult<UpdateUserTokensResponse>> ResetUserTokens(
        int userId,
        [FromBody] ResetUserTokensRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var adminId = GetCurrentUserId();
            request.UserId = userId;
            var response = await _adminService.ResetUserTokensAsync(adminId, request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error resetting tokens: {ex.Message}");
        }
    }

    /// <summary>
    /// GET /api/admin/users - Get all users (Admin/SuperAdmin only)
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserListItemResponse>>> GetAllUsers()
    {
        try
        {
            var adminId = GetCurrentUserId();
            var users = await _adminService.GetAllUsersAsync(adminId);
            return Ok(users);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving users: {ex.Message}");
        }
    }

    /// <summary>
    /// GET /api/admin/users/{userId} - Get specific user (Admin/SuperAdmin only)
    /// </summary>
    [HttpGet("users/{userId}")]
    public async Task<ActionResult<UserListItemResponse>> GetUserById(int userId)
    {
        try
        {
            var adminId = GetCurrentUserId();
            var user = await _adminService.GetUserByIdAsync(adminId, userId);
            return Ok(user);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving user: {ex.Message}");
        }
    }

    /// <summary>
    /// POST /api/admin/users/{userId}/assign-role - Assign role to user (SuperAdmin only)
    /// </summary>
    [HttpPost("users/{userId}/assign-role")]
    public async Task<ActionResult<AssignRoleResponse>> AssignRoleToUser(
        int userId,
        [FromBody] AssignRoleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var superAdminId = GetCurrentUserId();
            request.UserId = userId;
            var response = await _adminService.AssignRoleToUserAsync(superAdminId, request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error assigning role: {ex.Message}");
        }
    }
}