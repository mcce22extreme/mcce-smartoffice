using Mcce.SmartOffice.MobileApp.ViewModels;

namespace Mcce.SmartOffice.MobileApp.Pages;

public partial class BookingsPage : ContentPage
{
    public BookingsPage(BookingsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await ((BookingsViewModel)BindingContext).LoadBookings();
    }
}
