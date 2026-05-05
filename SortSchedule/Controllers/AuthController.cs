using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortSchedule.Application.Abstractions.Auth;
using SortSchedule.Application.Abstractions.User;
using SortSchedule.Application.DTOs.Auth;
using SortSchedule.Application.DTOs.User;
using SortSchedule.Controllers.Common;
using System.Security.Claims;

namespace SortSchedule.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService, IUserService userService) : BaseController
{
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        return await HandleActionAsync(_authService.LoginAsync(request, cancellationToken));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        return await HandleActionAsync(_authService.RefreshTokenAsync(request, cancellationToken));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(Shared.Common.Result<bool>.FailureResult("Invalid token.", "UNAUTHORIZED", System.Net.HttpStatusCode.Unauthorized));
        }

        return await HandleActionAsync(_userService.ChangePasswordAsync(userId, request, cancellationToken));
    }
}
