using Mcce.SmartOffice.App;
using MauiIcons.Fluent;
using Mcce.SmartOffice.AdminApp.Managers;
using Mcce.SmartOffice.AdminApp.Pages;
using Mcce.SmartOffice.AdminApp.ViewModels;

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
                .AddSingleton<IAccountManager, AccountManager>();

            // Register pages
            builder.Services
                .AddTransient<LoadingPage>()
                .AddTransient<LoginPage>()
                .AddTransient<MainPage>();

            // Register viewmodels
            builder.Services
                .AddTransient<LoadingViewModel>()
                .AddTransient<LoginViewModel>()
                .AddTransient<MainViewModel>();
            ;
        }
    }
}
