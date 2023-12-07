using Mcce.SmartOffice.MobileApp.ViewModels;

namespace Mcce.SmartOffice.MobileApp.Pages;

[QueryProperty(nameof(WorkspaceNumber), nameof(WorkspaceNumber))]
public partial class WorkspaceConfigurationDetailPage
{
    public string WorkspaceNumber { get; set; }

    public WorkspaceConfigurationDetailPage(WorkspaceConfigurationDetailViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        ((WorkspaceConfigurationDetailViewModel)BindingContext).WorkspaceNumber = WorkspaceNumber;

        base.OnNavigatedTo(args);
    }
}
