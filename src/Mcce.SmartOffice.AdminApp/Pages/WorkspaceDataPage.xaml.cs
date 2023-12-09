using Mcce.SmartOffice.AdminApp.ViewModels;

namespace Mcce.SmartOffice.AdminApp.Pages;

public partial class WorkspaceDataPage
{
    private readonly WorkspaceDataViewModel _viewModel;

    public WorkspaceDataPage(WorkspaceDataViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await _viewModel.Activate();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();

        await _viewModel.Deactivate();
    }
}
