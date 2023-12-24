using CommunityToolkit.Maui;
using IdentityModel.OidcClient;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.Common.Services;

namespace Mcce.SmartOffice.App
{
    public abstract class BootstrapBase<TApp> where TApp : Application
    {
        protected IAppConfig AppConfig { get; }

        public BootstrapBase(IAppConfig appConfig)
        {
            AppConfig = appConfig;
        }

        public MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
              .UseMauiApp<TApp>()
              .UseMauiCommunityToolkit()
              .ConfigureFonts(fonts =>
              {
                  fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                  fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
              });

            builder.Services.AddTransient<WebAuthenticatorBrowser>();

            if (AppConfig.AuthConfig != null)
            {
                builder.Services.AddTransient(sp => new OidcClient(new OidcClientOptions
                {
                    Authority = AppConfig.AuthConfig.AuthEndpoint,
                    ClientId = "smartoffice",
                    Scope = "openid profile",
                    RedirectUri = AppConfig.AuthConfig.AuthRedirectUri,
                    PostLogoutRedirectUri = AppConfig.AuthConfig.AuthRedirectUri,
                    Browser = sp.GetRequiredService<WebAuthenticatorBrowser>()
                }));
            }

            if (AppConfig.MqttConfig != null)
            {
                builder.Services.AddSingleton<IMessageService>(s => new MessageService(AppConfig.MqttConfig));
            }

            builder.Services.AddHttpClient();

            builder.Services
                .AddSingleton(AppConfig)
                .AddSingleton(Connectivity.Current)
                .AddSingleton(SecureStorage.Default);

            // Register service
            builder.Services
                .AddSingleton<INavigationService, NavigationService>()
                .AddSingleton<IDialogService, DialogService>()
                .AddSingleton<IAuthService, AuthService>();

            OnCreateMauiApp(builder);

            return builder.Build();
        }

        protected virtual void OnCreateMauiApp(MauiAppBuilder builder)
        {
        }
    }
}
