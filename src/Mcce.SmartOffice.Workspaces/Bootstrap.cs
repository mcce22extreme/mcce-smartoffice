using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.Workspaces.Managers;

namespace Mcce.SmartOffice.Workspaces
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(AppConfig.DbConfig, AppDbContext.DATABASE_SCHEMA);

            builder.Services.AddScoped<IWorkspaceManager, WorkspaceManager>();
        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            await app.Services.InitializeDatabase<AppDbContext>();
        }
    }
}
