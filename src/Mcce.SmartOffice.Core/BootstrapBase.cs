﻿using System.Reflection;
using FluentValidation;
using Mcce.SmartOffice.Core.Attributes;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Core.Enums;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.Core.Handlers;
using Mcce.SmartOffice.Core.Providers;
using Mcce.SmartOffice.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using Serilog;

namespace Mcce.SmartOffice.Core
{
    public abstract class BootstrapBase<T> where T : AppConfig
    {
        protected Assembly Assembly => Assembly.GetEntryAssembly();

        protected IConfiguration Configuration { get; }

        protected T AppConfig { get; }

        public BootstrapBase()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("SMARTOFFICE_")
                .Build();

            AppConfig = Configuration.Get<T>();
        }

        public async Task Run(string[] args)
        {
            // Configure serilog
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            ConfigureBuilder(builder);

            var app = builder.Build();

            await ConfigureApp(app);

            app.Run();
        }

        private void ConfigureBuilder(WebApplicationBuilder builder)
        {
            IdentityModelEventSource.ShowPII = true;

            var appInfo = new AppInfo(Assembly.GetName());

            Log.Information($"Starting {appInfo.AppName} v{appInfo.AppVersion}...");

            Log.Debug($"AppConfig: \n" + JsonConvert.SerializeObject(AppConfig, Formatting.Indented));

            builder.WebHost.UseUrls(AppConfig.BaseAddress);

            // Configure logging
            builder.Services.AddLogging(cfg =>
            {
                cfg.ClearProviders();
                cfg.AddSerilog(Log.Logger);
            });

            // Configure global filters
            builder.Services.AddControllersWithViews(opt =>
            {
                opt.Filters.Add(new TypeFilterAttribute(typeof(OperationLoggerAttribute)));
                opt.Filters.Add(new TypeFilterAttribute(typeof(OperationValidatorAttribute)));
            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IAppInfo>(appInfo);

            builder.Services.AddScoped<IValidationProvider, ValidationProvider>();

            builder.Services.RegisterAllTypes<IValidator>(new[] { Assembly });

            builder.Services.AddAutoMapper(Assembly);

            if (AppConfig.MqttConfig != null)
            {
                builder.Services.AddScoped<IMessageService>(s => new MessageService(AppConfig.MqttConfig));

                builder.Services.AddHostedService<MessageHandlerService>();

                builder.Services.RegisterAllTypes<IMessageHandler>(new[] { Assembly });
            }

            OnConfigureBuilder(builder);
        }

        protected virtual void OnConfigureBuilder(WebApplicationBuilder builder)
        {
        }

        private async Task ConfigureApp(WebApplication app)
        {
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.ConfigureExceptionHandler();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.MapDefaultControllerRoute();

            await OnConfigureApp(app);
        }

        protected virtual Task OnConfigureApp(WebApplication app)
        {
            return Task.CompletedTask;
        }
    }
}