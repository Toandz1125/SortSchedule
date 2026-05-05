using Microsoft.AspNetCore.Authorization;
using SortSchedule.Domain.Enums;

namespace Infrastructure.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Resource { get; }
    public PermissionAction Action { get; }

    public PermissionRequirement(string resource, PermissionAction action)
    {
        Resource = resource;
        Action = action;
    }
}
