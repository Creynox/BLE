using BLE.UI.ViewModels;
using UraniumUI.Pages;

namespace BLE.UI.Views;

public partial class NewProbePage : UraniumContentPage
{
    public NewProbePage(NewProbeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
