using Microsoft.AspNetCore.Authorization;
using SortSchedule.Application.Abstractions;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SortSchedule.Application.Abstractions.Auth;

namespace Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var permissions = await authService.GetUserPermissionsAsync(userId);
        
        var requiredPermission = $"{requirement.Resource}:{requirement.Action}";
        
        if (permissions != null && permissions.Contains(requiredPermission))
        {
            context.Succeed(requirement);
        }
    }
}
