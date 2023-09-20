using Mcce.SmartOffice.Bookings.Configs;
using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Bookings.Services;
using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Extensions;

namespace Mcce.SmartOffice.Bookings
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(AppConfig.DbConfig);

            builder.Services.AddScoped<IBookingManager, BookingManager>();

            builder.Services.AddSingleton<IEmailService>(s => new EmailService(AppConfig.EmailConfig));
        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            await app.Services.InitializeDatabase<AppDbContext>();
        }
    }
}
