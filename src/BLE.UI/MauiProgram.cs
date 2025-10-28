using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BLE.UI.Views;
// using Microsoft.EntityFrameworkCore; // falls du den DbContext hier konfigurierst

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

        // DbContext ggf. hier (oder an der Stelle, wo du es schon hast)
        // builder.Services.AddDbContext<BLE.Data.BLEDbContext>(opt =>
        //     opt.UseNpgsql(Configuration.GetConnectionString("Postgres")));

        // Seiten registrieren
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<DashboardPage>(); // ⬅️ wird oben per DI resolved

        return builder.Build();
    }
}
