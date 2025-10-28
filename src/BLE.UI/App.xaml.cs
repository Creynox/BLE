using Microsoft.Maui.Controls;
using BLE.UI.Views;

namespace BLE.UI;

public partial class App : Application
{
    public App(LoginPage loginPage)
    {
        InitializeComponent();
        MainPage = new NavigationPage(loginPage);
    }
}
