using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace BLE.UI.WinUI;

// kein partial, kein InitializeComponent, keine App.xaml nötig
public class App : Microsoft.Maui.MauiWinUIApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
