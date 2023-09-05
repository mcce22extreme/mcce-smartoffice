using Mcce.SmartOffice.Bookings.Configs;
using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Bookings.Services;
using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Bookings
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

            builder.Services.AddScoped<IBookingManager, BookingManager>();

            builder.Services.AddScoped<IEmailService>(s => new EmailService(AppConfig.EmailConfig));

        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}
