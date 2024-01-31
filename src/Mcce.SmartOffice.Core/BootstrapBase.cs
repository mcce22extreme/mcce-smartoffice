using System.Reflection;
using Azure.Identity;
using FluentValidation;
using Mcce.SmartOffice.Core.Accessors;
using Mcce.SmartOffice.Core.Attributes;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.Core.Handlers;
using Mcce.SmartOffice.Core.Providers;
using Mcce.SmartOffice.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;

namespace Mcce.SmartOffice.Core
{
    public abstract class BootstrapBase<T> where T : AppConfig
    {
        private static readonly string CORS_POLICY = "CORSPOLICY";

        protected Assembly Assembly => Assembly.GetEntryAssembly();

        protected IConfiguration Configuration { get; }

        protected T AppConfig { get; }

        public BootstrapBase()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("SMARTOFFICE_");

            var cfg = builder.Build();
            var appConfig = cfg.Get<AppConfig>();

            if (appConfig.AppConfigUrl.HasValue())
            {
                builder.AddAzureAppConfiguration(opt =>
                {
                    opt.Connect(new Uri(appConfig.AppConfigUrl), new DefaultAzureCredential());
                    opt.ConfigureKeyVault(kvopt =>
                    {
                        kvopt.SetCredential(new DefaultAzureCredential());
                    });
                });
            }

            Configuration = builder.Build();
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
            var appInfo = new AppInfo(Assembly.GetName());

            Log.Information($"Starting {appInfo.AppName} v{appInfo.AppVersion}...");

            Log.Debug($"AppConfig: \n" + JsonConvert.SerializeObject(AppConfig, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            }));

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
                opt.Filters.Add(new AuthorizeFilter(AuthConstants.APP_ROLE_USERS));
                opt.Filters.Add(new TypeFilterAttribute(typeof(OperationLoggerAttribute)));
                opt.Filters.Add(new TypeFilterAttribute(typeof(OperationValidatorAttribute)));
            }).AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            // Configure cors
            if (AppConfig.CorsConfig != null)
            {
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(CORS_POLICY, p =>
                    {
                        p.WithOrigins(AppConfig.CorsConfig.Origins);
                        p.AllowAnyMethod();
                        p.AllowAnyHeader();
                        p.AllowCredentials();
                    });
                });
            }

            // Configure authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.MetadataAddress = $"{AppConfig.AuthConfig.AuthUrl}/.well-known/openid-configuration";
                    opt.Authority = AppConfig.AuthConfig.AuthUrl;
                    opt.Audience = "account";
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                    };
                });

            // Configure authorization
            builder.Services.AddAuthorization(opt =>
            {
                opt.AddPolicy(AuthConstants.APP_ROLE_ADMINS, p => p.RequireRole(AuthConstants.APP_ROLE_ADMINS));
                opt.AddPolicy(AuthConstants.APP_ROLE_USERS, p => p.RequireRole(AuthConstants.APP_ROLE_ADMINS, AuthConstants.APP_ROLE_USERS));

            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(opt =>
            {
                var scheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{AppConfig.AuthConfig.AuthFrontendUrl}/protocol/openid-connect/auth"),
                            TokenUrl = new Uri($"{AppConfig.AuthConfig.AuthFrontendUrl}/protocol/openid-connect/token")
                        }
                    },
                    Type = SecuritySchemeType.OAuth2
                };

                opt.AddSecurityDefinition("OAuth", scheme);
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Id = "OAuth", Type = ReferenceType.SecurityScheme }
                        },
                        new List<string> { }
                    }
                });
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IAuthContextAccessor, AuthContextAccessor>();

            builder.Services.AddSingleton<IAppInfo>(appInfo);

            builder.Services.AddSingleton<IAppConfig>(AppConfig);

            builder.Services.AddSingleton<IValidationProvider, ValidationProvider>();

            builder.Services.RegisterAllTypes<IValidator>(new[] { Assembly });

            builder.Services.AddAutoMapper(Assembly);

            if (AppConfig.MqttConfig != null)
            {
                builder.Services.AddSingleton<IMessageService>(s => new MessageService(AppConfig.MqttConfig));

                builder.Services.AddHostedService<MessageHandlerService>();

                builder.Services.RegisterAllTypes<IMessageHandler>([Assembly]);
            }

            OnConfigureBuilder(builder);
        }

        protected virtual void OnConfigureBuilder(WebApplicationBuilder builder)
        {
        }

        private async Task ConfigureApp(WebApplication app)
        {
            app.UseRouting();

            app.UseCors(CORS_POLICY);

            app.UseAuthentication();

            app.UseAuthorization();

            app.ConfigureExceptionHandler();

            app.UseSwagger();

            app.UseSwaggerUI(opt =>
            {
                opt.OAuthClientId(AppConfig.AuthConfig.ClientId);
                opt.OAuthScopes("profile");
                opt.OAuthUsePkce();
            });

            app.MapDefaultControllerRoute();

            await OnConfigureApp(app);
        }

        protected virtual Task OnConfigureApp(WebApplication app)
        {
            return Task.CompletedTask;
        }
    }
}
