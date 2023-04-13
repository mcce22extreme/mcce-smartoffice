﻿using Mcce22.SmartOffice.Bookings.Managers;
using Mcce22.SmartOffice.Core;
using Mcce22.SmartOffice.Workspaces.Managers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace Mcce22.SmartOffice.Workspaces
{
    public class Bootstrap : BootstrapBase
    {
        protected override string ApiPrefix => "workspaceapi";

        protected override async Task ConfigureBuilder(WebApplicationBuilder builder)
        {
            await base.ConfigureBuilder(builder);

            var appSettings = Configuration.Get<AppSettings>();
            await appSettings.LoadConfigFromAWSSecretsManager();

            Log.Debug("Application Configuration:");
            Log.Debug(JsonConvert.SerializeObject(appSettings, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));

            builder.Services.AddAutoMapper(typeof(Bootstrap).Assembly);
            
#if DEBUG
            // Configure urls (only for local debugging)
            builder.WebHost.UseUrls(appSettings.BaseAddress);
#endif
            //builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("workspacedb"));
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(appSettings.ConnectionString));

            builder.Services.AddScoped<IWorkspaceManager, WorkspaceManager>();

            builder.Services.AddScoped<IWorkspaceConfigurationManager, WorkspaceConfigurationManager>();

            builder.Services.AddScoped<IWorkspaceDataManager, WorkspaceDataManager>();
        }

        protected override async Task ConfigureApp(WebApplication app)
        {
            await base.ConfigureApp(app);

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Check if we can migrate database automatically
            if (dbContext.Database.IsRelational())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}
