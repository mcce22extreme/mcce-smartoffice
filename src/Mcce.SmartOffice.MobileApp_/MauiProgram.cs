using System.Reflection;
using IdentityModel.OidcClient;
using MauiIcons.Fluent;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Pages;
using Mcce.SmartOffice.MobileApp.Services;
using Mcce.SmartOffice.MobileApp.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

            builder.Services.AddSingleton(config);

            builder.Services.AddSingleton(Connectivity.Current);

            builder.Services.AddSingleton(SecureStorage.Default);

            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<IAuthService, AuthService>();

            builder.Services.AddSingleton<IAccountManager, AccountManager>();

            builder.Services.AddSingleton<IBookingManager, BookingManager>();

            builder.Services.AddSingleton<IWorkspaceManager, WorkspaceManager>();

            builder.Services.AddTransient<LoadingPage>();

            builder.Services.AddTransient<LoadingViewModel>();

            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<MainPage>();

            builder.Services.AddTransient<MainViewModel>();

            builder.Services.AddTransient<BookingsPage>();

            builder.Services.AddTransient<BookingsViewModel>();

            builder.Services.AddTransient<CreateBookingPage>();

            builder.Services.AddTransient<CreateBookingViewModel>();

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
