﻿using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.WorkspaceConfigurations.Managers;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceConfigurations
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseCosmos(AppConfig.DbConfig.ConnectionString, AppConfig.DbConfig.DatabaseName));

            builder.Services.AddScoped<IWorkspaceConfigurationManager, WorkspaceConfigurationManager>();
        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            await app.Services.InitializeDatabase<AppDbContext>();
        }
    }
}
