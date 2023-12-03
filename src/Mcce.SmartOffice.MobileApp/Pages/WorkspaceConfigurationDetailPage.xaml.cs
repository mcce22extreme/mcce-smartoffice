using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.Client.Common;
using Mcce.SmartOffice.MobileApp.ViewModels;

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
