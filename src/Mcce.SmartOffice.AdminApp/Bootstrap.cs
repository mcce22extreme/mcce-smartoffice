using MauiIcons.Fluent;
using Mcce.SmartOffice.AdminApp.Managers;
using Mcce.SmartOffice.AdminApp.Pages;
using Mcce.SmartOffice.AdminApp.ViewModels;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Mcce.SmartOffice.AdminApp
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

            builder.UseSkiaSharp(true);

            // Register app shell
            builder.Services
                .AddSingleton<Shell, AppShell>()
                .AddTransient(s => Application.Current.Dispatcher.CreateTimer());

            // Register managers
            builder.Services
                .AddSingleton<IAccountManager, AccountManager>()
                .AddSingleton<IWorkspaceManager, WorkspaceManager>()
                .AddSingleton<IWorkspaceDataManager, WorkspaceDataManager>()
                .AddSingleton<IBookingManager, BookingManager>();

            // Register pages
            builder.Services
                .AddTransient<LoadingPage>()
                .AddTransient<LoginPage>()
                .AddTransient<MainPage>()
                .AddTransient<WorkspaceListPage>()
                .AddTransient<WorkspaceDetailPage>()
                .AddTransient<WorkspaceDataPage>()
                .AddTransient<BookingListPage>();

            // Register viewmodels
            builder.Services
                .AddTransient<LoadingViewModel>()
                .AddTransient<LoginViewModel>()
                .AddTransient<MainViewModel>()
                .AddTransient<WorkspaceListViewModel>()
                .AddTransient<WorkspaceDetailViewModel>()
                .AddTransient<WorkspaceDataViewModel>()
                .AddTransient<BookingListViewModel>();
            ;
        }
    }
}
