using Mcce.SmartOffice.Core.Exceptions;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.UserImages.Models;
using Mcce.SmartOffice.UserImages.Services;

namespace Mcce.SmartOffice.UserImages.Managers
{
    public interface IUserImageManager
    {
        Task<UserImageModel[]> GetUserImages(Func<string, string> urlFunc);

        Task<UserImageModel[]> GetUserImagesByUserName(string userName, Func<string, string> urlFunc);

        Task<Stream> GetUserImage(string imageKey);

        Task<UserImageModel> StoreUserImage(IFormFile file, Func<string, string> urlFunc);

        Task DeleteUserImage(string imageKey);
    }

    public class UserImageManager : IUserImageManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStorageService _storageService;

        public UserImageManager(IHttpContextAccessor contextAccessor, IStorageService storageService)
        {
            _contextAccessor = contextAccessor;
            _storageService = storageService;
        }

        public async Task<UserImageModel[]> GetUserImages(Func<string, string> urlFunc)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            return await GetUserImagesByUserName(currentUser.UserName, urlFunc);
        }

        public async Task<UserImageModel[]> GetUserImagesByUserName(string userName, Func<string, string> urlFunc)
        {
            var files = await _storageService.GetFiles(userName);

            return files
                .Select(x => new UserImageModel
                {
                    Url = urlFunc(Path.GetFileName(x)),
                })
                .ToArray();
        }

        public async Task<Stream> GetUserImage(string imageKey)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var path = Path.Combine(currentUser.UserName, imageKey);

            if (!await _storageService.FileExists(path))
            {
                throw new NotFoundException($"Could not find user image with key '{imageKey}'!");
            }

            return await _storageService.GetContent(path);
        }

        public async Task<UserImageModel> StoreUserImage(IFormFile file, Func<string, string> urlFunc)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var imageKey = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var path = Path.Combine(currentUser.UserName, imageKey);

            using var stream = file.OpenReadStream();

            await _storageService.StoreContent(path, stream);

            return new UserImageModel
            {
                Url = urlFunc(imageKey)
            };
        }

        public async Task DeleteUserImage(string imageKey)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var path = Path.Combine(currentUser.UserName, imageKey);

            if (!await _storageService.FileExists(path))
            {
                throw new NotFoundException($"Could not find user image with key '{imageKey}'!");
            }

            await _storageService.DeleteContent(path);
        }
    }
}
