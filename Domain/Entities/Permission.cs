using SortSchedule.Domain.Enums;

namespace SortSchedule.Domain.Entities;

public sealed class Permission
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Resource { get; set; } = string.Empty;

    public PermissionAction Action { get; set; }
}
