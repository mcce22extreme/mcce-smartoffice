using Mcce.SmartOffice.MobileApp.ViewModels;

namespace Mcce.SmartOffice.MobileApp.Pages;

public partial class LoadingPage : ContentPage
{
    public LoadingPage(LoadingViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await ((LoadingViewModel)BindingContext).VerifySession();
    }
}
