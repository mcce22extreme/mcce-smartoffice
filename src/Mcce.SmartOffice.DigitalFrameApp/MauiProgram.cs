using System.Reflection;
using Mcce.SmartOffice.App;
using Microsoft.Extensions.Configuration;

namespace Mcce.SmartOffice.DigitalFrameApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("Mcce.SmartOffice.DigitalFrameApp.appsettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build()
                .Get<AppConfig>();

            return new Bootstrap(config).CreateMauiApp();
        }
    }
}
