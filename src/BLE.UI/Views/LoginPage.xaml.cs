using BLE.UI.ViewModels;
using UraniumUI.Pages;

namespace BLE.UI.Views;

public partial class LoginPage : UraniumContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

