using Microsoft.EntityFrameworkCore;
using SortSchedule.Application.Abstractions.User;
using SortSchedule.Domain.Entities;
using SortSchedule.Domain.Enums;

namespace SortSchedule.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _dbContext.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email, ct);
    }

    public async Task<AppUser?> GetByIdWithRolesAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken ct = default)
    {
        return await _dbContext.Users.AnyAsync(x => x.Email == email, ct);
    }

    public Task AddAsync(AppUser user, CancellationToken ct = default)
    {
        return _dbContext.Users.AddAsync(user, ct).AsTask();
    }

    public async Task<AppRole?> GetRoleByNameAsync(string roleName, CancellationToken ct = default)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == roleName, ct);
    }

    public async Task<AppRole?> GetRoleByEnumAsync(RolesEnum roleEnum, CancellationToken ct = default)
    {
        var roleName = roleEnum.ToString();
        return await GetRoleByNameAsync(roleName, ct);
    }

    public async Task<RefreshToken?> GetActiveRefreshTokenByHashAsync(string tokenHash, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash
                    && x.RevokedAtUtc == null
                    && x.ExpiresAtUtc > now,
                ct);
    }

    public async Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        await _dbContext.RefreshTokens
            .Where(x => x.UserId == userId
                     && x.RevokedAtUtc == null
                     && x.ExpiresAtUtc > now)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.RevokedAtUtc, now), ct);

        _dbContext.ChangeTracker.Clear();
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<HashSet<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role!.Permissions)
            .Select(rp => new { rp.Permission!.Resource, rp.Permission.Action, rp.Effect })
            .ToListAsync(cancellationToken);

        var allowed = permissions
            .Where(p => p.Effect == PermissionEffect.Allow)
            .Select(p => $"{p.Resource}:{p.Action}")
            .ToHashSet();

        var denied = permissions
            .Where(p => p.Effect == PermissionEffect.Deny)
            .Select(p => $"{p.Resource}:{p.Action}")
            .ToHashSet();

        allowed.ExceptWith(denied);
        return allowed;
    }

    public Task UpdateAsync(AppUser user, CancellationToken ct = default)
    {
        _dbContext.Users.Update(user);
        return Task.CompletedTask;
    }
}
