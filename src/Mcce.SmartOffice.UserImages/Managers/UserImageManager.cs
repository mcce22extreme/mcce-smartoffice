using Mcce.SmartOffice.Core.Exceptions;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.UserImages.Entities;
using Mcce.SmartOffice.UserImages.Models;
using Mcce.SmartOffice.UserImages.Services;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.UserImages.Managers
{
    public interface IUserImageManager
    {
        Task<UserImageModel[]> GetUserImages();

        Task<UserImageModel[]> GetUserImagesByUserName(string userName);

        Task<Stream> GetUserImage(string imageKey);

        Task<UserImageModel> StoreUserImage(IFormFile file);

        Task DeleteUserImage(string imageKey);
    }

    public class UserImageManager : IUserImageManager
    {
        private readonly string _forntendUrl;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStorageService _storageService;

        public UserImageManager(string forntendUrl, AppDbContext dbContext, IHttpContextAccessor contextAccessor, IStorageService storageService)
        {
            _forntendUrl = forntendUrl;
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
            _storageService = storageService;
        }

        private string CreateImageUrl(string imageKey)
        {
            return $"{_forntendUrl}/{imageKey}";
        }

        public async Task<UserImageModel[]> GetUserImages()
        {
            var currentUser = _contextAccessor.GetUserInfo();

            return await GetUserImagesByUserName(currentUser.UserName);
        }

        public async Task<UserImageModel[]> GetUserImagesByUserName(string userName)
        {
            var userImages = await _dbContext.UserImages
                .Where(x => x.UserName == userName)
                .ToListAsync();

            return userImages
                .Select(x => new UserImageModel
                {
                    Url = CreateImageUrl(x.ImageKey),
                }).ToArray();
        }

        public async Task<Stream> GetUserImage(string imageKey)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            if (!await _storageService.FileExists(imageKey))
            {
                throw new NotFoundException($"Could not find user image with key '{imageKey}'!");
            }

            return await _storageService.GetContent(imageKey);
        }

        public async Task<UserImageModel> StoreUserImage(IFormFile file)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var imageKey = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            using var stream = file.OpenReadStream();

            await _storageService.StoreContent(imageKey, stream);

            await _dbContext.UserImages.AddAsync(new UserImage
            {
                UserName = currentUser.UserName,
                ImageKey = imageKey
            });

            await _dbContext.SaveChangesAsync();

            return new UserImageModel
            {
                Url = CreateImageUrl(imageKey)
            };
        }

        public async Task DeleteUserImage(string imageKey)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var userImages = await _dbContext.UserImages
                .Where(x => x.ImageKey == imageKey && x.UserName == currentUser.UserName)
                .ToListAsync();

            foreach (var userImage in userImages)
            {
                await _storageService.DeleteContent(imageKey);

                _dbContext.UserImages.Remove(userImage);
            }

            await _dbContext.SaveChangesAsync();

        }
    }
}
