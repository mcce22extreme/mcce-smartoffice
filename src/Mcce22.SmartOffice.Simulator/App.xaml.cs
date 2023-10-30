using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using Mcce.SmartOffice.Core.Services;
using Mcce22.SmartOffice.Simulator.Managers;
using Mcce22.SmartOffice.Simulator.Services;
using Mcce22.SmartOffice.Simulator.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mcce22.SmartOffice.Simulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var services = new ServiceCollection();

            services.AddSingleton(s => _appConfig);
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<IMessageService>(s => new MessageService(_appConfig.MqttConfig));
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IWorkspaceManager>(s => new WorkspaceManager(_appConfig.BaseAddress, s.GetRequiredService<HttpClient>()));
            services.AddSingleton<IAuthService, AuthService>();

            var serviceProvider = services.BuildServiceProvider();

            MainWindow = serviceProvider.GetService<MainWindow>();
            MainWindow.Show();
        }
    }
}
