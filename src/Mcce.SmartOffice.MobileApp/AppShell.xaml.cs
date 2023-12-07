using MauiIcons.Core;
using Mcce.SmartOffice.MobileApp.Pages;

namespace Mcce.SmartOffice.MobileApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            _ = new MauiIcon();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(BookingListPage), typeof(BookingListPage));
            Routing.RegisterRoute(nameof(BookingDetailPage), typeof(BookingDetailPage));
            Routing.RegisterRoute(nameof(UserImageListPage), typeof(UserImageListPage));
            Routing.RegisterRoute(nameof(WorkspaceConfigurationListPage), typeof(WorkspaceConfigurationListPage));
            Routing.RegisterRoute(nameof(WorkspaceConfigurationDetailPage), typeof(WorkspaceConfigurationDetailPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        }
    }
}
