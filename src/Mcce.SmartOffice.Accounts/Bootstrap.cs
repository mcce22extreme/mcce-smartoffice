using Mcce.SmartOffice.Accounts.Managers;
using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Configs;

namespace Mcce.SmartOffice.Accounts
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAccountManager, AccountManager>();
        }
    }
}
