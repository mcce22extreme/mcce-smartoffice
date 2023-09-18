using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Controls.Ribbon;
using HeyRed.Mime;
using Mcce22.SmartOffice.Client.Models;
using Newtonsoft.Json;

namespace Mcce22.SmartOffice.Client.Managers
{
    public interface IUserImageManager
    {
        Task<UserImageModel[]> GetList(string userId);

        Task Save(string userId, string filePath);

        Task Delete(string userImageId);
    }

    public class UserImageManager : IUserImageManager
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public UserImageManager(IAppConfig appConfig, HttpClient httpClient)

        {
            _baseUrl = $"{appConfig.BaseAddress}/userimage";
            _httpClient = httpClient;
        }

        public async Task<UserImageModel[]> GetList(string userId)
        {
            var url = $"{_baseUrl}/{userId}";
            var json = await _httpClient.GetStringAsync(url);

            var entries = JsonConvert.DeserializeObject<UserImageModel[]>(json);

            return entries;
        }

        public async Task Delete(string userImageId)
        {
            var url = $"{_baseUrl}/{userImageId}";

            await _httpClient.DeleteAsync(url);
        }

        public async Task Save(string userId, string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var content = new MultipartFormDataContent
            {
                {
                    new StreamContent(stream)
                    {
                        Headers =
                        {
                            ContentLength = stream.Length,
                            ContentType = new MediaTypeHeaderValue(MimeTypesMap.GetMimeType(filePath))
                        }
                    },
                    "File",
                    Path.GetFileName(filePath)
                }
            };

            await _httpClient.PostAsync($"{_baseUrl}/{userId}", content);
        }
    }
}
