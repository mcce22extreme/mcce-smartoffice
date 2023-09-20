using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.WorkspaceDataEntries.Generators;
using Mcce.SmartOffice.WorkspaceDataEntries.Managers;

namespace Mcce.SmartOffice.WorkspaceDataEntries
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(AppConfig.DbConfig);

            builder.Services.AddScoped<IWorkspaceDataEntryManager, WorkspaceDataEntryManager>();

            builder.Services.AddSingleton<IWeiGenerator, WeiGenerator>();
        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            await app.Services.InitializeDatabase<AppDbContext>();
        }
    }
}
