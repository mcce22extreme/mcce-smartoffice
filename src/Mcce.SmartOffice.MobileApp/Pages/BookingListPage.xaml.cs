using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.MobileApp.ViewModels;

namespace Mcce.SmartOffice.MobileApp.Pages;

public partial class BookingListPage : ContentPage
{
    public BookingListPage(BookingListViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await ((IViewModel)BindingContext).Activate();
    }
}
