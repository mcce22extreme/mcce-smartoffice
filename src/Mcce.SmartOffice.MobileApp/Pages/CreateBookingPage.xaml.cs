using Mcce.SmartOffice.MobileApp.ViewModels;

namespace Mcce.SmartOffice.MobileApp.Pages;

public partial class CreateBookingPage : ContentPage
{
    public CreateBookingPage(CreateBookingViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await ((CreateBookingViewModel)BindingContext).LoadWorkspaces();
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
        if(((CreateBookingViewModel)BindingContext).HasUnsavedData)
        {
            var deferral = e.GetDeferral();

            var result = await Application.Current.MainPage.DisplayAlert("Cancel Booking?", "Do you really want to cancel the booking?", "Yes", "No");

            if (result)
            {
                deferral.Complete();
            }
            else
            {
                e.Cancel();
            }
        }        
    }
}
