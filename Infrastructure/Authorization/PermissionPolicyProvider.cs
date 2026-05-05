using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SortSchedule.Domain.Enums;

namespace Infrastructure.Authorization;

public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith("Permission:", StringComparison.OrdinalIgnoreCase))
        {
            var parts = policyName.Split(':');
            if (parts.Length == 3)
            {
                var resource = parts[1];
                if (Enum.TryParse<PermissionAction>(parts[2], true, out var action))
                {
                    var policy = new AuthorizationPolicyBuilder();
                    policy.AddRequirements(new PermissionRequirement(resource, action));
                    return policy.Build();
                }
            }
        }

        return await base.GetPolicyAsync(policyName);
    }
}
