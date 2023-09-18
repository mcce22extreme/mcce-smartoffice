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

            builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
                .AddEnvironmentVariables(EnvironmentVariables.PREFIX);

            // Configure serilog
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            var appInfo = new AppInfo(Assembly.GetEntryAssembly().GetName());

            Log.Information($"Starting {appInfo.AppName} v{appInfo.AppVersion}...");

            Log.Debug($"Configuration: \n" + JsonConvert.SerializeObject(
                Environment.GetEnvironmentVariables()
                .Keys
                .OfType<string>()
                .Where(x => x.StartsWith(EnvironmentVariables.PREFIX))
                .ToDictionary(x => x, x => Environment.GetEnvironmentVariable(x)), Formatting.Indented));

            var baseAddress = Environment.GetEnvironmentVariable(EnvironmentVariables.BASEADDRESS);

            builder.Services.AddLogging(cfg =>
            {
                cfg.ClearProviders();
                cfg.AddSerilog(Log.Logger);
            });

            builder.WebHost.UseUrls(baseAddress);

            builder.Services.AddSwaggerForOcelot(builder.Configuration);

            builder.Services.AddHttpClient();

            builder.Services.AddOcelot(builder.Configuration);

            builder.Services.AddMvcCore();

            builder.Services.AddEndpointsApiExplorer();


            builder.Services.ConfigureRoutePlaceholders();

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
