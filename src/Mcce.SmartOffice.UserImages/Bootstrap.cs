using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.UserImages.Configs;
using Mcce.SmartOffice.UserImages.Enums;
using Mcce.SmartOffice.UserImages.Managers;
using Mcce.SmartOffice.UserImages.Services;

namespace Mcce.SmartOffice.UserImages
{
    public class Bootstrap : BootstrapBase<AppConfig>
    {
        protected override void OnConfigureBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserImageManager, UserImageManager>();

            switch (AppConfig.StorageConfig.StorageType)
            {
                case StorageType.FileSystem:
                    builder.Services.AddScoped<IStorageService, FileSystemStorageService>(s => new FileSystemStorageService(AppConfig.StorageConfig.Path));
                    break;
                default:
                    throw new InvalidOperationException($"The storage type '{AppConfig.StorageConfig.StorageType}' is not supported!");
            }
        }
    }
}
