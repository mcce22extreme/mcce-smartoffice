using Mcce.SmartOffice.MobileApp.ViewModels;

namespace Mcce.SmartOffice.MobileApp.Pages;

public partial class UserImagesPage : ContentPage
{
    public UserImagesPage(UserImagesViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await ((UserImagesViewModel)BindingContext).LoadUserImages();
    }
}
