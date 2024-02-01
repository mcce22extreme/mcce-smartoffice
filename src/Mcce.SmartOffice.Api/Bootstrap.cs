using System.Reflection;
using AutoMapper;
using Azure.Identity;
using FluentValidation;
using Mcce.SmartOffice.Api.Accessors;
using Mcce.SmartOffice.Api.Attributes;
using Mcce.SmartOffice.Api.Configs;
using Mcce.SmartOffice.Api.Constants;
using Mcce.SmartOffice.Api.Extensions;
using Mcce.SmartOffice.Api.Generators;
using Mcce.SmartOffice.Api.Handlers;
using Mcce.SmartOffice.Api.Managers;
using Mcce.SmartOffice.Api.Providers;
using Mcce.SmartOffice.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;

namespace Mcce.SmartOffice.Api
{
    public class Bootstrap
    {
        private static readonly string CORS_POLICY = "CORSPOLICY";

        protected Assembly Assembly => Assembly.GetEntryAssembly();

        protected IConfiguration Configuration { get; }

        protected AppConfig AppConfig { get; }

        public Bootstrap()
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
            AppConfig = Configuration.Get<AppConfig>();
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

            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(AppConfig.ConnectionString));

            builder.Services.AddScoped<IBookingManager>(s => new BookingManager(
                AppConfig.FrontendUrl?.TrimEnd('/'),
               s.GetRequiredService<AppDbContext>(),
               s.GetRequiredService<IMapper>(),
               s.GetRequiredService<IAuthContextAccessor>(),
               s.GetRequiredService<IMessageService>()));

            builder.Services.AddScoped<IAccountManager, AccountManager>();

            builder.Services.AddScoped<IUserImageManager>(s => new UserImageManager(
                AppConfig.FrontendUrl?.TrimEnd('/'),
                s.GetRequiredService<AppDbContext>(),
                s.GetRequiredService<IAuthContextAccessor>(),
                s.GetRequiredService<IStorageService>()));

            builder.Services.AddScoped<IStorageService, FileSystemStorageService>(s => new FileSystemStorageService(AppConfig.StoragePath));

            builder.Services.AddScoped<IWorkspaceConfigurationManager, WorkspaceConfigurationManager>();

            builder.Services.AddScoped<IWorkspaceDataEntryManager, WorkspaceDataEntryManager>();

            builder.Services.AddSingleton<IWeiGenerator, WeiGenerator>();

            builder.Services.AddScoped<IWorkspaceManager, WorkspaceManager>();
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

            await app.Services.InitializeDatabase<AppDbContext>();
        }
    }
}
