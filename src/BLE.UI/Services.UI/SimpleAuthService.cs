using System;
using System.Threading;
using System.Threading.Tasks;
using BLE.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLE.UI.Services.UI;

public class SimpleAuthService : ISimpleAuthService
{
    private readonly IDbContextFactory<BLEDbContext> _dbFactory;
    private readonly PasswordHasher<ApplicationUser> _passwordHasher;
    private readonly UserSession _session;

    public SimpleAuthService(
        IDbContextFactory<BLEDbContext> dbFactory,
        PasswordHasher<ApplicationUser> passwordHasher,
        UserSession session)
    {
        _dbFactory = dbFactory;
        _passwordHasher = passwordHasher;
        _session = session;
    }

    public async Task<ApplicationUser?> SignInAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

        var normalized = username.Trim().ToUpperInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalized, cancellationToken);
        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return null;
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded)
        {
            if (verification == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, password);
                await db.SaveChangesAsync(cancellationToken);
            }

            _session.SetCurrentUser(user);
            return user;
        }

        return null;
    }

    public Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        _session.SetCurrentUser(null);
        return Task.CompletedTask;
    }
}

