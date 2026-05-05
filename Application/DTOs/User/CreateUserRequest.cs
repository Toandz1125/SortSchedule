using SortSchedule.Domain.Enums;

namespace SortSchedule.Application.DTOs.User;

public sealed class CreateUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public RolesEnum Role { get; init; }
}
