using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using CefSharp;
using CefSharp.Handler;
using CefSharp.Wpf;
using Mcce22.SmartOffice.Client.Managers;
using Mcce22.SmartOffice.Client.Services;
using Mcce22.SmartOffice.Client.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mcce22.SmartOffice.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        private readonly IConfiguration _configuration;
        private readonly IAppConfig _appConfig;

        public App()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
                .Build();

            _appConfig = _configuration.Get<AppConfig>();
        }

        private void OnStartUp(object sender, StartupEventArgs e)
        {
            InitCefSharp();

            var services = new ServiceCollection();

            services.AddSingleton(s => _appConfig);

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<MainWindow>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<WorkspaceListViewModel>();
            services.AddSingleton<BookingListViewModel>();
            services.AddSingleton<WorkspaceConfigurationListViewModel>();
            services.AddSingleton<UserImageListViewModel>();
            services.AddSingleton<SeedDataViewModel>();
            services.AddSingleton<CreateBookingViewModel>();
            services.AddSingleton<WorkspaceDataListViewModel>();
            services.AddSingleton<ConfigViewModel>();

            services.AddSingleton<IWorkspaceManager, WorkspaceManager>();
            services.AddSingleton<IBookingManager, BookingManager>();
            services.AddSingleton<IWorkspaceConfigurationManager, WorkspaceConfigurationManager>();
            services.AddSingleton<IUserImageManager, UserImageManager>();
            services.AddSingleton<IWorkspaceDataEntryManager, WorkspaceDataEntryManager>();
            services.AddSingleton<IAccountManager, AccountManager>();
            services.AddSingleton<IConfigManager, ConfigManager>();

            services.AddSingleton<HttpClient>();

            services.AddSingleton<IAuthService, AuthService>();

            _serviceProvider = services.BuildServiceProvider();

            MainWindow = _serviceProvider.GetService<MainWindow>();
            MainWindow.Show();
        }

        private void InitCefSharp()
        {
            const bool multiThreadedMessageLoop = true;

            IBrowserProcessHandler browserProcessHandler;

            if (multiThreadedMessageLoop)
            {
                browserProcessHandler = new BrowserProcessHandler();
            }            

            var settings = new CefSettings();
            settings.MultiThreadedMessageLoop = multiThreadedMessageLoop;
            settings.ExternalMessagePump = !multiThreadedMessageLoop;
            settings.RootCachePath = Path.GetFullPath("cache");
            settings.CachePath = Path.GetFullPath("cache\\global");

            var result = Cef.Initialize(settings, true, browserProcessHandler);
        }
    }
}
