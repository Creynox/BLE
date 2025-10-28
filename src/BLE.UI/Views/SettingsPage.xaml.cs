using BLE.UI.ViewModels;
using Microsoft.Maui.Controls;
using UraniumUI.Pages;

namespace BLE.UI.Views;

public partial class SettingsPage : UraniumContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        _viewModel.ToggleThemeCommand.Execute(null);
    }
}
