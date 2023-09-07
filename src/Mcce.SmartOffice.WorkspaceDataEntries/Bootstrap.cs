using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.WorkspaceDataEntries.Generators;
using Mcce.SmartOffice.WorkspaceDataEntries.Managers;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceDataEntries
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(AppConfig.DbConfig.ConnectionString));

            builder.Services.AddScoped<IWorkspaceDataEntryManager, WorkspaceDataEntryManager>();

            builder.Services.AddScoped<IWeiGenerator, WeiGenerator>();
        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            await app.Services.InitializeDatabase<AppDbContext>();
        }
    }
}
