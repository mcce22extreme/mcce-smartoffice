using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HeyRed.Mime;
using Mcce22.SmartOffice.Client.Models;

namespace Mcce22.SmartOffice.Client.Managers
{
    public interface IUserImageManager
    {
        Task<UserImageModel[]> GetList();

        Task Save(string filePath);

        Task Delete(string userImageId);
    }

    public class UserImageManager : ManagerBase<UserImageModel>, IUserImageManager
    {
        public UserImageManager(IAppConfig appConfig, HttpClient httpClient)
            : base($"{appConfig.BaseAddress}/userimage", httpClient)

        {
        }

        public async Task Save(string filePath)
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

            await HttpClient.PostAsync($"{BaseUrl}", content);
        }
    }
}
