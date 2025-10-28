using System;
using BLE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        // Resolve dashboard via DI (statt new DashboardPage(_sp))
        var dashboard = scope.ServiceProvider.GetRequiredService<DashboardPage>();
        await Navigation.PushAsync(dashboard);
    }
}
