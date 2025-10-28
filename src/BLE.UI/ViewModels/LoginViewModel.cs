using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BLE.UI.Services.UI;
using Microsoft.Maui.Controls;

namespace BLE.UI.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly ISimpleAuthService _authService;
    private readonly UserSession _session;
    private readonly ToastService _toastService;

    private string _username = string.Empty;
    private string _password = string.Empty;

    public LoginViewModel(ISimpleAuthService authService, UserSession session, ToastService toastService)
    {
        _authService = authService;
        _session = session;
        _toastService = toastService;

        Title = "Anmeldung";
        LoginCommand = new Command(async () => await ExecuteLoginAsync(), () => !IsBusy);
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }

    private async Task ExecuteLoginAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ((Command)LoginCommand).ChangeCanExecute();

        try
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await _toastService.ShowAsync("Bitte Benutzername und Passwort eingeben.");
                return;
            }

            var user = await _authService.SignInAsync(Username, Password);
            if (user is null)
            {
                await _toastService.ShowAsync("Anmeldung fehlgeschlagen.");
                return;
            }

            await _toastService.ShowAsync($"Willkommen {user.DisplayName ?? user.UserName}!");
            await Shell.Current.GoToAsync("//dashboard");
            Username = string.Empty;
            Password = string.Empty;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await _toastService.ShowAsync("Anmeldung nicht moeglich. Bitte spaeter erneut versuchen.");
        }
        finally
        {
            IsBusy = false;
            ((Command)LoginCommand).ChangeCanExecute();
        }
    }
}
