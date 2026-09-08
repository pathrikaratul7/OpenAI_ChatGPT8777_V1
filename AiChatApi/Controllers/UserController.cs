using AiChatApi.Models;
using AiChatApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AiChatApi.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
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

    /// <summary>
    /// GET /api/users/profile - Get current user's profile (requires JWT token)
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileResponse>> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var profile = await _userService.GetUserProfileAsync(userId);
            return Ok(profile);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving profile: {ex.Message}");
        }
    }

    /// <summary>
    /// GET /api/users/subscription-status - Get subscription and token usage status (requires JWT token)
    /// </summary>
    [HttpGet("subscription-status")]
    public async Task<ActionResult<SubscriptionStatusResponse>> GetSubscriptionStatus()
    {
        try
        {
            var userId = GetCurrentUserId();
            var status = await _userService.GetSubscriptionStatusAsync(userId);
            return Ok(status);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving subscription status: {ex.Message}");
        }
    }

    /// <summary>
    /// GET /api/users/token-usage-history?limit=50 - Get token usage history (requires JWT token)
    /// </summary>
    [HttpGet("token-usage-history")]
    public async Task<ActionResult<IEnumerable<TokenUsageLogResponse>>> GetTokenUsageHistory([FromQuery] int limit = 50)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (limit < 1 || limit > 500)
            {
                return BadRequest("Limit must be between 1 and 500.");
            }

            var history = await _userService.GetTokenUsageHistoryAsync(userId, limit);
            return Ok(history);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving token usage history: {ex.Message}");
        }
    }
}