using System.Net;
using System.Reflection;
using IdentityModel.OidcClient;
using Mcce22.SmartOffice.MobileApp.Constants;
using Mcce22.SmartOffice.MobileApp.Managers;
using Mcce22.SmartOffice.MobileApp.Pages;
using Mcce22.SmartOffice.MobileApp.Services;
using Mcce22.SmartOffice.MobileApp.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mcce22.SmartOffice.MobileApp
{
    public static class MauiProgram
    {
        private static IAppConfig LoadConfig()
        {
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("Mcce22.SmartOffice.MobileApp.appsettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            return config.Get<AppConfig>();
        }

        public static MauiApp CreateMauiApp()
        {
            var config = LoadConfig();

            var builder = MauiApp.CreateBuilder();
            builder
              .UseMauiApp<App>()
              .ConfigureFonts(fonts =>
              {
                  fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                  fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
              });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton(config);
            builder.Services.AddSingleton(Connectivity.Current);
            builder.Services.AddSingleton(SecureStorage.Default);

            //var handler = new HttpClientHandler();
            //handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
            //{
            //    return true;
            //};

            //builder.Services.AddHttpClient("default")
            //    .ConfigurePrimaryHttpMessageHandler(() => handler);

            builder.Services.AddHttpClient();

            //Register services
            builder.Services.AddSingleton<IAuthService, AuthService>();

            // Register viewmodels
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<AccountViewModel>();

            // Register pages
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<AccountPage>();

            // Register managers
            builder.Services.AddSingleton<IAccountManager, AccountManager>();

            builder.Services.AddTransient<WebAuthenticatorBrowser>();

            builder.Services.AddTransient(sp => new OidcClient(new OidcClientOptions
            {
                Authority = config.AuthEndpoint,
                ClientId = "smartoffice",
                Scope = "openid profile",
                RedirectUri = "smartofficemobileapp://callback",
                PostLogoutRedirectUri = "smartofficemobileapp://",
                Browser = sp.GetRequiredService<WebAuthenticatorBrowser>()
            }));

            // Register routes
            Routing.RegisterRoute(Routes.Main, typeof(MainPage));
            Routing.RegisterRoute(Routes.Account, typeof(AccountPage));

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            return builder.Build();
        }
    }
}
