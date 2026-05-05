using Microsoft.AspNetCore.Authorization;
using SortSchedule.Domain.Enums;

namespace Infrastructure.Authorization;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string resource, PermissionAction action) 
        : base(policy: $"Permission:{resource}:{action}")
    {
    }
}
