using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortSchedule.Application.Abstractions.User;
using SortSchedule.Application.DTOs.User;
using SortSchedule.Controllers.Common;
using Infrastructure.Authorization;
using SortSchedule.Domain.Enums;

namespace SortSchedule.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UserController(IUserService userService) : BaseController
{
    private readonly IUserService _userService = userService;

    [HasPermission("User", PermissionAction.Create)]
    [HttpPost]
    public async Task<IActionResult> CreateUsers([FromBody] List<CreateUserRequest> requests, CancellationToken cancellationToken)
    {
        return await HandleActionAsync(_userService.CreateUsersAsync(requests, cancellationToken));
    }
}
