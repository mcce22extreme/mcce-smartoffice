using Mcce22.SmartOffice.MobileApp.ViewModels;

namespace Mcce22.SmartOffice.MobileApp.Pages;

public partial class AccountPage : ContentPage
{
    public AccountPage(AccountViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        await ((AccountViewModel)BindingContext).Load();
    }
}
