using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Core.Enums;
using Mcce.SmartOffice.Workspaces.Managers;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Workspaces
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            switch (AppConfig.DbConfig.DbType)
            {
                case DbType.SQLite:
                    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(AppConfig.DbConfig.ConnectionString));
                    break;
                default:
                    throw new InvalidOperationException($"The database type '{AppConfig.DbConfig.DbType}' is not supported!");
            }            

            builder.Services.AddScoped<IWorkspaceManager, WorkspaceManager>();
        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}
