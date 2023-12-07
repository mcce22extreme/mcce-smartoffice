using Mcce.SmartOffice.AdminApp.ViewModels;

namespace Mcce.SmartOffice.AdminApp.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
