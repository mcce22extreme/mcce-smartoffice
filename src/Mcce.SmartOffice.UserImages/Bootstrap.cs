using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.UserImages.Configs;
using Mcce.SmartOffice.UserImages.Managers;
using Mcce.SmartOffice.UserImages.Services;

namespace Mcce.SmartOffice.UserImages
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserImageManager, UserImageManager>();

            builder.Services.AddScoped<IStorageService, FileSystemStorageService>(s => new FileSystemStorageService(AppConfig.StoragePath));
        }
    }
}
