using Mcce.SmartOffice.AdminApp.ViewModels;
using Mcce.SmartOffice.App.ViewModels;

namespace Mcce.SmartOffice.AdminApp.Pages;

public partial class WorkspaceListPage : ContentPage
{
    public WorkspaceListPage(WorkspaceListViewModel viewModel)
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
