using BLE.UI.ViewModels;
using UraniumUI.Pages;

namespace BLE.UI.Views;

public partial class DashboardPage : UraniumContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}

