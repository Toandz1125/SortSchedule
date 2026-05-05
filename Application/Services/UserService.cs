using FluentValidation;
using Shared.Common;
using SortSchedule.Application.Abstractions.Auth;
using SortSchedule.Application.Abstractions.User;
using SortSchedule.Application.DTOs.User;
using SortSchedule.Domain.Entities;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SortSchedule.Application.Services;

public sealed class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IValidator<CreateUserRequest> createValidator,
    IValidator<ChangePasswordRequest> changePasswordValidator) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IValidator<CreateUserRequest> _createValidator = createValidator;
    private readonly IValidator<ChangePasswordRequest> _changePasswordValidator = changePasswordValidator;

    private async Task<Result<CreateUserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var exists = await _userRepository.ExistsAsync(normalizedEmail, ct);
        if (exists)
        {
            return Result<CreateUserResponse>.FailureResult($"Email '{normalizedEmail}' is already in use.", "EMAIL_ALREADY_EXISTS", HttpStatusCode.Conflict);
        }

        var role = await _userRepository.GetRoleByEnumAsync(request.Role, ct);
        if (role is null)
        {
            return Result<CreateUserResponse>.FailureResult($"Role '{request.Role}' does not exist.", "ROLE_NOT_FOUND", HttpStatusCode.BadRequest);
        }

        var rawPassword = GenerateRandomPassword();
        var user = new AppUser
        {
            Email = normalizedEmail,
            UserName = request.UserName.Trim(),
            PasswordHash = _passwordHasher.Hash(rawPassword)
        };

        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            User = user,
            Role = role
        });

        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        return Result<CreateUserResponse>.SuccessResult(new CreateUserResponse
        {
            Email = user.Email,
            UserName = user.UserName,
            Role = request.Role,
            GeneratedPassword = rawPassword
        }, "User created successfully", HttpStatusCode.Created);
    }

    public async Task<Result<List<CreateUserResponse>>> CreateUsersAsync(List<CreateUserRequest> requests, CancellationToken ct = default)
    {
        if (requests == null || requests.Count == 0)
        {
            return Result<List<CreateUserResponse>>.FailureResult("Request list is empty.");
        }

        var responses = new List<CreateUserResponse>();
        var errors = new List<string>();

        foreach (var req in requests)
        {
            try
            {
                var result = await CreateUserAsync(req, ct);
                if (result.Success)
                {
                    responses.Add(result.Data);
                }
                else
                {
                    errors.Add($"Failed to create user {req.Email}: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error creating user {req.Email}: {ex.Message}");
            }
        }

        if (responses.Count == 0 && errors.Count > 0)
        {
            return Result<List<CreateUserResponse>>.FailureResult(string.Join(" | ", errors), "BATCH_FAILED", HttpStatusCode.BadRequest);
        }

        return Result<List<CreateUserResponse>>.SuccessResult(responses, $"Created {responses.Count} users. Errors: {errors.Count}", HttpStatusCode.Created);
    }

    public async Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        await _changePasswordValidator.ValidateAndThrowAsync(request, ct);

        var user = await _userRepository.GetByIdWithRolesAsync(userId, ct);
        if (user is null)
        {
            return Result<bool>.FailureResult("User not found.", "USER_NOT_FOUND", HttpStatusCode.NotFound);
        }

        if (!_passwordHasher.Verify(request.OldPassword, user.PasswordHash))
        {
            return Result<bool>.FailureResult("Incorrect old password.", "INVALID_PASSWORD", HttpStatusCode.BadRequest);
        }

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        await _userRepository.SaveChangesAsync(ct);
        await _userRepository.RevokeAllRefreshTokensAsync(userId, ct); // Force re-login
        //await _userRepository.UpdateAsync(user, ct);

        return Result<bool>.SuccessResult(true, "Password changed successfully.");
    }

    private static string GenerateRandomPassword(int length = 10)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
        var res = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create())
        {
            var uintBuffer = new byte[4];
            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(uintBuffer);
                uint num = BitConverter.ToUInt32(uintBuffer, 0);
                res.Append(validChars[(int)(num % (uint)validChars.Length)]);
            }
        }
        return res.ToString();
    }
    // impl in future 
    public Task<Result<bool>> DeleteUser(Guid userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
