using Mcce.SmartOffice.MobileApp.ViewModels;
using Microsoft.Maui.Platform;

namespace Mcce.SmartOffice.MobileApp.Pages;

[QueryProperty(nameof(WorkspaceNumber), nameof(WorkspaceNumber))]
public partial class WorkspaceConfigurationDetailPage : ContentPage
{
    public string WorkspaceNumber { get; set; }

    public WorkspaceConfigurationDetailPage(WorkspaceConfigurationDetailViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        ((WorkspaceConfigurationDetailViewModel)BindingContext).WorkspaceNumber = WorkspaceNumber;

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
        if (((WorkspaceConfigurationDetailViewModel)BindingContext).HasUnsavedData)
        {
            var deferral = e.GetDeferral();

            var result = await Application.Current.MainPage.DisplayAlert("Cancel?", "Do you really want to cancel?", "Yes", "No");

            if (result)
            {
                deferral.Complete();

#if ANDROID
                Platform.CurrentActivity.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
#endif
            }
            else
            {
                e.Cancel();
            }
        }
    }
}
