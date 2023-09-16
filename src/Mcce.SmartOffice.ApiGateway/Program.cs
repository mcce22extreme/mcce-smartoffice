using System.Reflection;
using Mcce.SmartOffice.ApiGateway.Constants;
using Mcce.SmartOffice.ApiGateway.Extensions;
using Mcce.SmartOffice.Core;
using Newtonsoft.Json;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

namespace Mcce.SmartOffice.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
                .Build();

            // Configure serilog
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            var appInfo = new AppInfo(Assembly.GetEntryAssembly().GetName());
            var appConfig = builder.Configuration.Get<AppConfig>();

            Log.Information($"Starting {appInfo.AppName} v{appInfo.AppVersion}...");

            Log.Debug($"AppConfig: \n" + JsonConvert.SerializeObject(appConfig, Formatting.Indented));

            builder.Services.AddLogging(cfg =>
            {
                cfg.ClearProviders();
                cfg.AddSerilog(Log.Logger);
            });

            builder.WebHost.UseUrls(appConfig.BaseAddress);

            builder.Services.AddSwaggerForOcelot(builder.Configuration);

            builder.Services.AddHttpClient();

            builder.Services.AddOcelot(builder.Configuration);

            builder.Services.AddMvcCore();

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseAuthentication();

            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseOcelot();

            app.Run();
        }
    }
}
