using Mcce.SmartOffice.AdminApp.ViewModels;
using Mcce.SmartOffice.App.ViewModels;

namespace Mcce.SmartOffice.AdminApp.Pages;

public partial class BookingListPage : ContentPage
{
    private readonly BookingListViewModel _viewModel;

    public BookingListPage(BookingListViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await ((IViewModel)BindingContext).Activate();
    }
}
