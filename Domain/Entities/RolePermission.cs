using SortSchedule.Domain.Enums;

namespace SortSchedule.Domain.Entities;

public sealed class RolePermission
{
    public Guid RoleId { get; init; }
    public AppRole Role { get; init; } = null!;

    public Guid PermissionId { get; init; }
    public Permission Permission { get; init; } = null!;

    public PermissionEffect Effect { get; set; } = PermissionEffect.Allow;
}
