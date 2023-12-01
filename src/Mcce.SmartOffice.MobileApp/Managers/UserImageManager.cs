using System.Net.Http.Headers;
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
        public UserImageManager(IAppConfig appConfig, IHttpClientFactory httpClientFactory, ISecureStorage secureStorage)
            : base(appConfig, httpClientFactory, secureStorage)
        {
        }

        public async Task<UserImageModel[]> GetUserImages()
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}userimage";

            var json = await httpClient.GetStringAsync(url);

            var userImages = JsonConvert.DeserializeObject<UserImageModel[]>(json);

            return userImages;
        }

        public async Task AddUserImage(Stream stream, string filePath, string contentType)
        {
            using var httpClient = await CreateHttpClient();

            using var content = new MultipartFormDataContent
            {
                {
                    new StreamContent(stream)
                    {
                        Headers =
                        {
                            ContentLength = stream.Length,
                            ContentType = new MediaTypeHeaderValue(contentType)
                        }
                    },
                    "File",
                    Path.GetFileName(filePath)
                }
            };                        

            var response = await httpClient.PostAsync($"{AppConfig.BaseAddress}userimage", content);
        }

        public async Task DeleteUserImage(string imageKey)
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}userimage/{imageKey}";

            var response = await httpClient.DeleteAsync(url);
        }
    }
}
