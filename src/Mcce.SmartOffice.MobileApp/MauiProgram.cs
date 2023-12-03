using System.Reflection;
using IdentityModel.OidcClient;
using MauiIcons.Fluent;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Pages;
using Mcce.SmartOffice.MobileApp.Services;
using Mcce.SmartOffice.MobileApp.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace Mcce.SmartOffice.MobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var config = LoadConfig();

            var builder = MauiApp.CreateBuilder();
            builder
              .UseMauiApp<App>()
              .UseFluentMauiIcons()
              .UseMauiCommunityToolkit()
              .ConfigureFonts(fonts =>
              {
                  fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                  fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
              });


#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<WebAuthenticatorBrowser>();

            builder.Services.AddTransient(sp => new OidcClient(new OidcClientOptions
            {
                Authority = config.AuthEndpoint,
                ClientId = "smartoffice",
                Scope = "openid profile",
                RedirectUri = "smartofficemobileapp://callback",
                PostLogoutRedirectUri = "smartofficemobileapp://callback",
                Browser = sp.GetRequiredService<WebAuthenticatorBrowser>()
            }));

            builder.Services.AddHttpClient();

            builder.Services
                .AddTransient<AppShell>()
                .AddSingleton(config)
                .AddSingleton(Connectivity.Current)
                .AddSingleton(SecureStorage.Default);

            // Register service
            builder.Services
                .AddSingleton<INavigationService, NavigationService>()
                .AddSingleton<IDialogService, DialogService>()
                .AddSingleton<IAuthService, AuthService>();

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

            //builder.Services.AddTransient<LoadingViewModel>();

            //builder.Services.AddTransient<LoginPage>();

            //builder.Services.AddTransient<LoginViewModel>();

            //builder.Services.AddTransient<LoginPage>();

            //builder.Services.AddTransient<MainPage>();

            //builder.Services.AddTransient<MainViewModel>();

            //builder.Services.AddTransient<BookingsPage>();

            //builder.Services.AddTransient<BookingsViewModel>();

            //builder.Services.AddTransient<CreateBookingPage>();

            //builder.Services.AddTransient<CreateBookingViewModel>();

            //builder.Services.AddTransient<UserImagesPage>();

            //builder.Services.AddTransient<UserImagesViewModel>();

            //builder.Services.AddTransient<WorkspaceConfigurationsPage>();

            //builder.Services.AddTransient<WorkspaceConfigurationsViewModel>();

            //builder.Services.AddTransient<WorkspaceConfigurationDetailPage>();

            //builder.Services.AddTransient<WorkspaceConfigurationDetailViewModel>();

            return builder.Build();
        }

        private static IAppConfig LoadConfig()
        {
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("Mcce.SmartOffice.MobileApp.appsettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            return config.Get<AppConfig>();
        }

    }
}
