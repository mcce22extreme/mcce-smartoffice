using MauiIcons.Fluent;
using Mcce.SmartOffice.AdminApp.Managers;
using Mcce.SmartOffice.AdminApp.Pages;
using Mcce.SmartOffice.AdminApp.ViewModels;
using Mcce.SmartOffice.App;

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

            // Register app shell
            builder.Services                
                .AddSingleton<Shell, AppShell>();

            // Register managers
            builder.Services
                .AddSingleton<IAccountManager, AccountManager>()
                .AddSingleton<IWorkspaceManager, WorkspaceManager>();

            // Register pages
            builder.Services
                .AddTransient<LoadingPage>()
                .AddTransient<LoginPage>()
                .AddTransient<MainPage>()
                .AddTransient<WorkspaceListPage>()
                .AddTransient<WorkspaceDetailPage>();

            // Register viewmodels
            builder.Services
                .AddTransient<LoadingViewModel>()
                .AddTransient<LoginViewModel>()
                .AddTransient<MainViewModel>()
                .AddTransient<WorkspaceListViewModel>()
                .AddTransient<WorkspaceDetailViewModel>();
            ;
        }
    }
}
