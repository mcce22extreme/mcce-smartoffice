using MauiIcons.Fluent;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Pages;
using Mcce.SmartOffice.MobileApp.ViewModels;

namespace Mcce.SmartOffice.MobileApp
{
    public class Bootstrap : BootstrapBase<App>
    {
        public Bootstrap(IAppConfig appConfig)
            : base(appConfig)
        {
        }

        protected override void OnCreateMauiApp(MauiAppBuilder builder)
        {
            builder.UseFluentMauiIcons();

            // Register app shell
            builder.Services                
                .AddSingleton<Shell, AppShell>();

            // Register managers
            builder.Services
                .AddSingleton<IAccountManager, AccountManager>()
                .AddSingleton<IBookingManager, BookingManager>()
                .AddSingleton<IWorkspaceManager, WorkspaceManager>()
                .AddSingleton<IUserImageManager, UserImageManager>()
                .AddSingleton<IWorkspaceConfigurationManager, WorkspaceConfigurationManager>();

            // Register pages
            builder.Services
                .AddTransient<LoadingPage>()
                .AddTransient<LoginPage>()
                .AddTransient<MainPage>()
                .AddTransient<BookingListPage>()
                .AddTransient<BookingDetailPage>()
                .AddTransient<UserImageListPage>()
                .AddTransient<WorkspaceConfigurationListPage>()
                .AddTransient<WorkspaceConfigurationDetailPage>();

            // Register viewmodels
            builder.Services
                .AddTransient<LoadingViewModel>()
                .AddTransient<LoginViewModel>()
                .AddTransient<MainViewModel>()
                .AddTransient<BookingListViewModel>()
                .AddTransient<BookingDetailViewModel>()
                .AddTransient<UserImageListViewModel>()
                .AddTransient<WorkspaceConfigurationListViewModel>()
                .AddTransient<WorkspaceConfigurationDetailViewModel>();
        }
    }
}
