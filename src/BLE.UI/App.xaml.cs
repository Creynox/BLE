
using System;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace BLE.UI;

public partial class App : Application
{
    public App(IServiceProvider sp)
    {
        InitializeComponent();
        MainPage = new NavigationPage(new Views.LoginPage(sp));
    }
}

