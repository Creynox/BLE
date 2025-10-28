using System;
using BLE.UI.Services.UI;
using BLE.UI.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace BLE.UI;

public partial class AppShell : Shell
{
    private readonly UserSession _session;
    private readonly ToastService _toastService;

    public AppShell(UserSession session, ToastService toastService)
    {
        InitializeComponent();

        _session = session;
        _toastService = toastService;

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        Routing.RegisterRoute(nameof(ProbenPage), typeof(ProbenPage));
        Routing.RegisterRoute(nameof(NewProbePage), typeof(NewProbePage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

        _session.UserChanged += OnUserChanged;
        Navigating += OnShellNavigating;

        UpdateSettingsVisibility();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (!_session.IsAuthenticated)
            {
                await GoToAsync(nameof(LoginPage));
            }
        });
    }

    private void UpdateSettingsVisibility()
    {
        if (SettingsShellContent is not null)
        {
            SettingsShellContent.IsVisible = _session.IsAdmin;

            if (!_session.IsAdmin)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (CurrentPage is SettingsPage)
                    {
                        await GoToAsync("//dashboard");
                    }
                });
            }
        }
    }

    private async void OnUserChanged(object? sender, EventArgs e)
    {
        UpdateSettingsVisibility();

        if (!_session.IsAuthenticated)
        {
            await MainThread.InvokeOnMainThreadAsync(async () => await GoToAsync(nameof(LoginPage)));
        }
    }

    private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        if (_session.IsAdmin)
        {
            return;
        }

        var target = e.Target?.Location?.OriginalString ?? string.Empty;
        if (string.IsNullOrEmpty(target))
        {
            return;
        }

        var normalized = target.Trim('/').ToLowerInvariant();
        if (normalized.StartsWith("settings") || normalized.Contains("settings"))
        {
            e.Cancel();
            await _toastService.ShowAsync("Keine Berechtigung.");
        }
    }
}
