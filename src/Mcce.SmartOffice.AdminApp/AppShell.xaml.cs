using MauiIcons.Core;
using Mcce.SmartOffice.AdminApp.Pages;

namespace Mcce.SmartOffice.AdminApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            _ = new MauiIcon();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(WorkspaceListPage), typeof(WorkspaceListPage));
            Routing.RegisterRoute(nameof(WorkspaceDetailPage), typeof(WorkspaceDetailPage));
            Routing.RegisterRoute(nameof(WorkspaceDataPage), typeof(WorkspaceDataPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));            
        }
    }
}
