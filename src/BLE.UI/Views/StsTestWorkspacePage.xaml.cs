using System.Threading.Tasks;
using BLE.UI.ViewModels;
using UraniumUI.Pages;

namespace BLE.UI.Views;

public partial class StsTestWorkspacePage : UraniumContentPage
{
    private readonly StsTestWorkspaceViewModel _viewModel;

    public StsTestWorkspacePage(StsTestWorkspaceViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await EnsureLoadedAsync();
    }

    private async Task EnsureLoadedAsync()
    {
        await _viewModel.LoadAsync();
    }
}
