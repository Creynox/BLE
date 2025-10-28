using BLE.UI.ViewModels;
using UraniumUI.Pages;

namespace BLE.UI.Views;

public partial class ProbenPage : UraniumContentPage
{
    private readonly ProbenViewModel _viewModel;

    public ProbenPage(ProbenViewModel viewModel)
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
