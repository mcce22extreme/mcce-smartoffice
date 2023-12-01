using Mcce.SmartOffice.MobileApp.Pages;

namespace Mcce.SmartOffice.MobileApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(BookingsPage), typeof(BookingsPage));
            Routing.RegisterRoute(nameof(CreateBookingPage), typeof(CreateBookingPage));
        }
    }
}
