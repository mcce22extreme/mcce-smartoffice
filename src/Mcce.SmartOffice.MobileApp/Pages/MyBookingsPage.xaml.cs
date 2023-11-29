using Mcce.SmartOffice.MobileApp.ViewModels;

namespace Mcce.SmartOffice.MobileApp.Pages;

public partial class MyBookingsPage : ContentPage
{
    public MyBookingsPage(MyBookingsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await ((MyBookingsViewModel)BindingContext).LoadBookings();
    }
}
