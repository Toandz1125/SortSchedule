using Shared.Common;
using SortSchedule.Application.DTOs.User;

namespace SortSchedule.Application.Abstractions.User;

public interface IUserService
{
    Task<Result<List<CreateUserResponse>>> CreateUsersAsync(List<CreateUserRequest> requests, CancellationToken ct = default);
    Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteUser (Guid userId, CancellationToken ct = default);
}
