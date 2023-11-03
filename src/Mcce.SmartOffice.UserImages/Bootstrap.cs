using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.UserImages.Configs;
using Mcce.SmartOffice.UserImages.Managers;
using Mcce.SmartOffice.UserImages.Services;

namespace Mcce.SmartOffice.UserImages
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(AppConfig.DbConfig, AppDbContext.DATABASE_SCHEMA);

            builder.Services.AddScoped<IUserImageManager>(s => new UserImageManager(
                AppConfig.FrontendUrl?.TrimEnd('/'),
                s.GetRequiredService<AppDbContext>(),
                s.GetRequiredService<IHttpContextAccessor>(),
                s.GetRequiredService<IStorageService>()));

            builder.Services.AddScoped<IStorageService, FileSystemStorageService>(s => new FileSystemStorageService(AppConfig.StoragePath));
        }

        protected override async Task OnConfigureApp(WebApplication app)
        {
            await app.Services.InitializeDatabase<AppDbContext>();
        }
    }
}
