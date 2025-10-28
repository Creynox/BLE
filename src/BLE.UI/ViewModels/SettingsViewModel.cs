using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BLE.UI.Services.UI;
using BLE.UI.Views;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace BLE.UI.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly UserSession _session;
    private readonly ISimpleAuthService _authService;
    private readonly ToastService _toastService;

    private bool _useDarkTheme;

    public SettingsViewModel(UserSession session, ISimpleAuthService authService, ToastService toastService)
    {
        _session = session;
        _authService = authService;
        _toastService = toastService;

        Title = "Einstellungen";

        _useDarkTheme = Application.Current?.RequestedTheme == AppTheme.Dark;

        ToggleThemeCommand = new Command(async () => await ApplyThemeAsync());
        SignOutCommand = new Command(async () => await SignOutAsync());
    }

    public bool UseDarkTheme
    {
        get => _useDarkTheme;
        set => SetProperty(ref _useDarkTheme, value);
    }

    public ICommand ToggleThemeCommand { get; }

    public ICommand SignOutCommand { get; }

    private Task ApplyThemeAsync()
    {
        Application.Current!.UserAppTheme = UseDarkTheme ? AppTheme.Dark : AppTheme.Light;
        return _toastService.ShowAsync($"Theme auf {(UseDarkTheme ? "dunkel" : "hell")} gesetzt.");
    }

    private async Task SignOutAsync()
    {
        try
        {
            await _authService.SignOutAsync();
            await _toastService.ShowAsync("Abgemeldet.");
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await _toastService.ShowAsync("Abmeldung fehlgeschlagen.");
        }
    }
}
