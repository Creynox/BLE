using Microsoft.Maui.Controls;

namespace BLE.UI;

public partial class App : Application
{
    public App(AppShell shell)
    {
        InitializeComponent();
        MainPage = shell;
    }
}
