using Mcce.SmartOffice.AdminApp.ViewModels;
using Mcce.SmartOffice.App.Pages;

namespace Mcce.SmartOffice.AdminApp.Pages;

[QueryProperty(nameof(WorkspaceNumber), nameof(WorkspaceNumber))]
public partial class WorkspaceDetailPage : DetailContentPage
{
    public string WorkspaceNumber { get; set; }

    public WorkspaceDetailPage(WorkspaceDetailViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
