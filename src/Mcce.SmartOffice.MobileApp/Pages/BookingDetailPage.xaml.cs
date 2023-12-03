using Mcce.SmartOffice.MobileApp.Helpers;
using Mcce.SmartOffice.MobileApp.ViewModels;

namespace Mcce.SmartOffice.MobileApp.Pages;

public partial class BookingDetailPage : ContentPage
{
    public BookingDetailPage(BookingDetailViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await ((IViewModel)BindingContext).Activate();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.Current.Navigating += OnNavigating;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        Shell.Current.Navigating -= OnNavigating;
    }

    private async void OnNavigating(object sender, ShellNavigatingEventArgs e)
    {
        var deferral = e.GetDeferral();

        if (await ((IDetailViewModelBase)BindingContext).CanGoBack())
        {
            deferral.Complete();

            PlatformHelpers.HideKeyboard();
        }
        else
        {
            e.Cancel();
        }
    }
}
