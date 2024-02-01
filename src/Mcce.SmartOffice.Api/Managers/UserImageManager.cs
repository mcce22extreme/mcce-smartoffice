using HeyRed.Mime;
using Mcce.SmartOffice.Api.Accessors;
using Mcce.SmartOffice.Api.Entities;
using Mcce.SmartOffice.Api.Exceptions;
using Mcce.SmartOffice.Api.Models;
using Mcce.SmartOffice.Api.Services;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace Mcce.SmartOffice.Api.Managers
{
    public interface IUserImageManager
    {
        Task<UserImageModel[]> GetUserImages();

        Task<UserImageModel[]> GetUserImagesByUserName(string userName);

        Task<Stream> GetUserImage(string imageKey);

        Task<UserImageModel> StoreUserImage(Stream stream, string mimeType);

        Task DeleteUserImage(string imageKey);
    }

    public class UserImageManager : IUserImageManager
    {
        private readonly string _frontendUrl;
        private readonly AppDbContext _dbContext;
        private readonly IAuthContextAccessor _contextAccessor;
        private readonly IStorageService _storageService;

        public UserImageManager(string frontendUrl, AppDbContext dbContext, IAuthContextAccessor contextAccessor, IStorageService storageService)
        {
            _frontendUrl = frontendUrl;
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
            _storageService = storageService;
        }

        private string CreateImageUrl(string imageKey)
        {
            return $"{_frontendUrl}/userimage/{imageKey}";
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
                    ImageKey = x.ImageKey,
                    Url = CreateImageUrl(x.ImageKey),
                    ThumbnailUrl = CreateImageUrl(x.ThumbnailImageKey)
                }).ToArray();
        }

        public async Task<Stream> GetUserImage(string imageKey)
        {
            if (!await _storageService.FileExists(imageKey))
            {
                throw new NotFoundException($"Could not find user image with key '{imageKey}'!");
            }

            return await _storageService.GetContent(imageKey);
        }

        public async Task<UserImageModel> StoreUserImage(Stream stream, string mimeType)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var fileExtension = MimeTypesMap.GetExtension(mimeType);

            var imageKey = $"{Guid.NewGuid()}.{fileExtension}";
            var thumbnailImageKey = $"{Guid.NewGuid()}_thumbnail.{fileExtension}";

            await _storageService.StoreContent(imageKey, stream);

            // Generate thumbnail if neccessary
            using var imageStream = await _storageService.GetContent(imageKey);
            using var sourceBitmap = SKBitmap.Decode(imageStream);

            if (sourceBitmap.Width > 800 || sourceBitmap.Height > 600)
            {
                var thumbnailSize = CalculateThumbnailSize(sourceBitmap.Width, sourceBitmap.Height, 800);

                using var scaledBitmap = sourceBitmap.Resize(new SKImageInfo(thumbnailSize.Width, thumbnailSize.Height), SKFilterQuality.Medium);
                using var scaledImage = SKImage.FromBitmap(scaledBitmap);
                using var data = scaledImage.Encode();

                using var thumbnailStream = new MemoryStream();

                data.SaveTo(thumbnailStream);

                thumbnailStream.Seek(0, SeekOrigin.Begin);

                await _storageService.StoreContent(thumbnailImageKey, thumbnailStream);
            }
            else
            {
                // Image is already small enough => use same image for thumbnail
                thumbnailImageKey = imageKey;
            }

            await _dbContext.UserImages.AddAsync(new UserImage
            {
                UserName = currentUser.UserName,
                ImageKey = imageKey,
                ThumbnailImageKey = thumbnailImageKey,
            });

            await _dbContext.SaveChangesAsync();

            return new UserImageModel
            {
                ImageKey = imageKey,
                Url = CreateImageUrl(imageKey),
                ThumbnailUrl = CreateImageUrl(thumbnailImageKey)
            };
        }

        private (int Width, int Height) CalculateThumbnailSize(int originalWidth, int originalHeight, int targetWidth)
        {
            // Calculate the aspect ratio
            double aspectRatio = (double)originalWidth / originalHeight;

            // Calculate the corresponding height based on the target width and aspect ratio
            int targetHeight = (int)(targetWidth / aspectRatio);

            return (targetWidth, targetHeight);
        }

        public async Task DeleteUserImage(string imageKey)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var userImages = await _dbContext.UserImages
                .Where(x => x.ImageKey == imageKey && x.UserName == currentUser.UserName)
                .ToListAsync();

            foreach (var userImage in userImages)
            {
                await _storageService.DeleteContent(userImage.ImageKey);
                await _storageService.DeleteContent(userImage.ThumbnailImageKey);

                _dbContext.UserImages.Remove(userImage);
            }

            await _dbContext.SaveChangesAsync();

        }
    }
}
