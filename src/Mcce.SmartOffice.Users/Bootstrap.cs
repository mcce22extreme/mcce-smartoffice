using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Users.Managers;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Users
{
    public class Bootstrap : BootstrapBase
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(AppConfig.ConnectionString));

            builder.Services.AddScoped<IUserManager, UserManager>();
        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}
