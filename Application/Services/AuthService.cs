using FluentValidation;
using SortSchedule.Application.Abstractions.Auth;
using SortSchedule.Application.DTOs.Auth;
using SortSchedule.Domain.Entities;
using Shared.Common;
using System.Net;
using SortSchedule.Application.Abstractions.User;

namespace SortSchedule.Application.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPasswordHasher passwordHasher,

    IValidator<LoginRequest> loginValidator,
    IValidator<RefreshTokenRequest> refreshValidator) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    private readonly IValidator<LoginRequest> _loginValidator = loginValidator;
    private readonly IValidator<RefreshTokenRequest> _refreshValidator = refreshValidator;

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        try
        {
            await _loginValidator.ValidateAndThrowAsync(request, ct);

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail, ct);
            if (user is null)
            {
                return Result<AuthResponse>.FailureResult("Invalid email or password.", "INVALID_CREDENTIALS", HttpStatusCode.Unauthorized);
            }

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return Result<AuthResponse>.FailureResult("Invalid email or password.", "INVALID_CREDENTIALS", HttpStatusCode.Unauthorized);
            }

            await _userRepository.RevokeAllRefreshTokensAsync(user.Id, ct);

            user.LastLoginAtUtc = DateTime.UtcNow;

            var roles = user.UserRoles.Select(static ur => ur.Role.Name).ToArray();
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var (rawToken, tokenHash) = _tokenService.GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = tokenHash,
                UserId = user.Id,
                User = user,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
            });

            await _userRepository.SaveChangesAsync(ct);

            var enumRoles = roles.Select(r => Enum.Parse<SortSchedule.Domain.Enums.RolesEnum>(r)).ToArray();

            return Result<AuthResponse>.SuccessResult(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = rawToken,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
                UserName = user.UserName,
                Email = user.Email,
                Roles = enumRoles
            });


        }
        catch (ValidationException)
        {
            return Result<AuthResponse>.FailureResult("Validation failed.", "VALIDATION_ERROR", HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
             return Result<AuthResponse>.FailureResult(ex.Message, "INTERNAL_ERROR", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        await _refreshValidator.ValidateAndThrowAsync(request, ct);

        var (_, inputHash) = BuildHashOnly(request.RefreshToken);
        var currentToken = await _userRepository.GetActiveRefreshTokenByHashAsync(inputHash, ct);
        if (currentToken is null)
        {
            return Result<AuthResponse>.FailureResult("Refresh token is invalid or expired.", "INVALID_REFRESH_TOKEN", HttpStatusCode.Unauthorized);
        }

        currentToken.RevokedAtUtc = DateTime.UtcNow;

        var user = await _userRepository.GetByIdWithRolesAsync(currentToken.UserId, ct);
        if (user is null)
        {
            return Result<AuthResponse>.FailureResult("User no longer exists.", "USER_NOT_FOUND", HttpStatusCode.Unauthorized);
        }

        var roles = user.UserRoles.Select(static ur => ur.Role.Name).ToArray();
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var (newRawToken, newHash) = _tokenService.GenerateRefreshToken();

        user.RefreshTokens.Add(new RefreshToken
        {
            TokenHash = newHash,
            UserId = user.Id,
            User = user,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        });

        await _userRepository.SaveChangesAsync(ct);

        var enumRoles = roles.Select(r => Enum.Parse<SortSchedule.Domain.Enums.RolesEnum>(r)).ToArray();

        return Result<AuthResponse>.SuccessResult(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRawToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(30),
            UserName = user.UserName,
            Email = user.Email,
            Roles = enumRoles
        });
    }



    private (string RawToken, string TokenHash) BuildHashOnly(string rawToken)
    {
        var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken)));
        return (rawToken, hash);
    }

    public async Task<HashSet<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetPermissionsAsync(userId, cancellationToken);
    }
}
