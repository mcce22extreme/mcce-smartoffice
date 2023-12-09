using System.Net.Http.Headers;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.MobileApp.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.MobileApp.Managers
{
    public interface IUserImageManager
    {
        Task<UserImageModel[]> GetUserImages();

        Task AddUserImage(Stream stream, string filePath, string contentType);

        Task DeleteUserImage(string imageKey);
    }

    public class UserImageManager : ManagerBase, IUserImageManager
    {
        public UserImageManager(
            IAppConfig appConfig,
            IHttpClientFactory httpClientFactory,
            ISecureStorage secureStorage,
            IAuthService authService)
            : base(appConfig, httpClientFactory, secureStorage, authService )
        {
        }

        public Task<UserImageModel[]> GetUserImages()
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}userimage";

                var json = await httpClient.GetStringAsync(url);

                var userImages = JsonConvert.DeserializeObject<UserImageModel[]>(json);

                return userImages;
            });
        }

        public Task AddUserImage(Stream stream, string filePath, string contentType)
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}userimage?fileName={Path.GetFileName(filePath)}";
                var streamContent = new StreamContent(stream)
                {
                    Headers =
                {
                    ContentLength = stream.Length,
                    ContentType = new MediaTypeHeaderValue(contentType)
                }
                };

                var response = await httpClient.PostAsync(url, streamContent);

                response.EnsureSuccessStatusCode();

                return Task.CompletedTask;
            });
        }

        public Task DeleteUserImage(string imageKey)
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}userimage/{imageKey}";

                var response = await httpClient.DeleteAsync(url);

                response.EnsureSuccessStatusCode();

                return Task.CompletedTask;
            });
        }
    }
}
