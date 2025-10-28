using System;
using System.IO;
using BLE.Data;
using BLE.Services.Etl;
using BLE.Services.Reporting;
using BLE.Services.Rules;
using BLE.UI.Services.UI;
using BLE.UI.ViewModels;
using BLE.UI.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using UraniumUI;
using UraniumUI.Material;

namespace BLE.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dataDirectory = Path.Combine(localAppData, "BLE");
        Directory.CreateDirectory(dataDirectory);
        var dbPath = Path.Combine(dataDirectory, "ble.db");
        var connectionString = $"Data Source={dbPath}";

        builder.Services.AddDbContextFactory<BLEDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<BLEDbContext>>().CreateDbContext());

        builder.Services.AddSingleton<PasswordHasher<ApplicationUser>>();

        builder.Services.AddScoped<ISimpleAuthService, SimpleAuthService>();
        builder.Services.AddSingleton<UserSession>();
        builder.Services.AddSingleton<ToastService>();

        builder.Services.AddScoped<EtlService>();
        builder.Services.AddScoped<PdfService>();
        builder.Services.AddScoped<RulesService>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ProbenViewModel>();
        builder.Services.AddTransient<NewProbeViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ProbenPage>();
        builder.Services.AddTransient<NewProbePage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddSingleton<AppShell>();

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<BLEDbContext>>().CreateDbContext();
        db.Database.EnsureCreated();

        var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher<ApplicationUser>>();

        EnsureUser("admin", "Administrator", "admin123!", true);
        EnsureUser("user", "Fachanwender", "user123!", false);

        void EnsureUser(string username, string displayName, string password, bool isAdmin)
        {
            var normalized = username.ToUpperInvariant();
            var existing = db.Users.FirstOrDefault(u => u.NormalizedUserName == normalized);
            if (existing is null)
            {
                var account = new ApplicationUser
                {
                    UserName = username,
                    NormalizedUserName = normalized,
                    DisplayName = displayName,
                    IsAdmin = isAdmin
                };
                account.PasswordHash = hasher.HashPassword(account, password);
                db.Users.Add(account);
            }
            else if (existing.IsAdmin != isAdmin || existing.DisplayName != displayName)
            {
                existing.IsAdmin = isAdmin;
                existing.DisplayName = displayName;
            }
        }

        db.SaveChanges();

        return app;
    }
}
