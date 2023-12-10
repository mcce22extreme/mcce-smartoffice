using AutoMapper;
using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Common.Services;
using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Accessors;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Core.Extensions;

namespace Mcce.SmartOffice.Bookings
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(AppConfig.DbConfig, AppDbContext.DATABASE_SCHEMA);

            builder.Services.AddScoped<IBookingManager>(s => new BookingManager(
                AppConfig.FrontendUrl?.TrimEnd('/'),
                s.GetRequiredService<AppDbContext>(),
                s.GetRequiredService<IMapper>(),
                s.GetRequiredService<IAuthContextAccessor>(),
                s.GetRequiredService<IMessageService>()));
        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            await app.Services.InitializeDatabase<AppDbContext>();
        }
    }
}
