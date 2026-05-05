using SortSchedule.Domain.Enums;

namespace SortSchedule.Application.DTOs.User;

public sealed class CreateUserResponse
{
    public string Email { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public RolesEnum Role { get; init; }
    public string GeneratedPassword { get; init; } = string.Empty;
}
