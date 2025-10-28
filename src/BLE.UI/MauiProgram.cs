using System;
using System.IO;
using System.Linq;
using BLE.Data;
using BLE.Services.Etl;
using BLE.Services.Reporting;
using BLE.UI.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace BLE.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ble.db");
        builder.Services.AddDbContext<BLEDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        builder.Services.AddScoped<EtlService>();
        builder.Services.AddScoped<PdfService>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BLEDbContext>();
            db.Database.EnsureCreated();

            if (!db.Users.Any())
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    DisplayName = "Administrator"
                };

                var hasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = hasher.HashPassword(user, "admin123!");
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        return app;
    }
}
