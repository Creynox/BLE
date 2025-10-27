using BLE.Data;
using BLE.Services.Etl;
using BLE.Services.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace BLE.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ble.db");
        builder.Services.AddDbContext<BLEDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));
        builder.Services.AddScoped<EtlService>();
        builder.Services.AddScoped<PdfService>();

        // Apply EF migrations on startup
        using (var scope = builder.Services.BuildServiceProvider().CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BLEDbContext>();
            db.Database.Migrate();
        }

        return builder.Build();
    }
}

