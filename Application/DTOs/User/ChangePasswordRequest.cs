namespace SortSchedule.Application.DTOs.User;

public sealed class ChangePasswordRequest
{
    public string OldPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
