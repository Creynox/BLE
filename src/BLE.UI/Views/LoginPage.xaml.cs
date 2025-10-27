using BLE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;

namespace BLE.UI.Views;

public partial class LoginPage : ContentPage
{
    private readonly IServiceProvider _sp;

    public LoginPage(IServiceProvider sp)
    {
        InitializeComponent();
        _sp = sp;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Ensure admin user exists (seed)
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BLEDbContext>();
        if (!await db.Users.AnyAsync())
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "admin", NormalizedUserName = "ADMIN", DisplayName = "Administrator" };
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>();
            user.PasswordHash = hasher.HashPassword(user, "admin123!");
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = _userEntry.Text?.Trim() ?? string.Empty;
        var password = _passEntry.Text ?? string.Empty;

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BLEDbContext>();
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            _status.Text = "Benutzer nicht gefunden";
            return;
        }
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash!, password);
        if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
        {
            _status.Text = "Passwort falsch";
            return;
        }

        await Navigation.PushAsync(new DashboardPage(_sp));
    }
}

