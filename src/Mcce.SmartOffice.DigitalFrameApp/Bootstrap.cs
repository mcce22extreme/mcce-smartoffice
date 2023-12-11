using CommunityToolkit.Maui;
using MauiIcons.Fluent;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.DigitalFrameApp.Managers;
using Mcce.SmartOffice.DigitalFrameApp.Pages;
using Mcce.SmartOffice.DigitalFrameApp.ViewModels;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcce.SmartOffice.DigitalFrameApp
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
            builder.UseMauiCommunityToolkit();

            // Register app shell
            builder.Services
                .TryAddSingleton<Shell, AppShell>();

            builder.Services
                .AddTransient(s => Application.Current.Dispatcher.CreateTimer());

            // Register managers
            builder.Services
                .AddSingleton<ISessionManager, SessionManager>()
                .AddSingleton<IWorkspaceDataManager, WorkspaceDataManager>();

            // Register pages
            builder.Services
                .AddTransient<MainPage>()
                .AddTransient<PrepareSessionPage>()
                .AddTransient<SlideshowPage>()
                .AddTransient<EndSessionPage>();

            // Register viewmodels
            builder.Services
                .AddTransient<MainViewModel>()
                .AddTransient<PrepareSessionViewModel>()
                .AddTransient<SlideshowViewModel>()
                .AddTransient<EndSessionViewModel>();
        }
    }
}
