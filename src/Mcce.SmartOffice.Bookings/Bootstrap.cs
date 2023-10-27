using AutoMapper;
using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.Core.Services;

namespace Mcce.SmartOffice.Bookings
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(AppConfig.DbConfig);

            builder.Services.AddScoped<IBookingManager>(s => new BookingManager(
                AppConfig.FrontendUrl?.TrimEnd('/'),
                s.GetRequiredService<AppDbContext>(),
                s.GetRequiredService<IMapper>(),
                s.GetRequiredService<IHttpContextAccessor>(),
                s.GetRequiredService<IMessageService>()));
        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            await app.Services.InitializeDatabase<AppDbContext>();
        }
    }
}
