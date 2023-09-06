﻿using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.UserImages.Managers;
using Mcce.SmartOffice.UserImages.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.UserImages.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserImageController : ControllerBase
    {
        private readonly IUserImageManager _userImageManager;

        public UserImageController(IUserImageManager userImageManager)
        {
            _userImageManager = userImageManager;
        }

        [HttpGet]
        public async Task<UserImageModel[]> GetUserImages()
        {
            return await _userImageManager.GetUserImages(CreateUrl);
        }

        [HttpGet("{imageKey}", Name = nameof(GetUserImage))]
        public async Task<Stream> GetUserImage(string imageKey)
        {
            return await _userImageManager.GetUserImage(imageKey);
        }

        [HttpPost]
        public async Task<UserImageModel> StoreUserImage(IFormFile file)
        {
            return await _userImageManager.StoreUserImage(file, CreateUrl);
        }

        [HttpDelete("{imageKey}")]
        public async Task DeleteUserImage(string imageKey)
        {
            await _userImageManager.DeleteUserImage(imageKey);
        }

        private string CreateUrl(string imageKey)
        {
            string referer = Request.Headers["Referer"];

            if (referer.HasValue())
            {
                var builder = new UriBuilder(referer);

                return $"{builder.Scheme}://{builder.Host}:{builder.Port}{Url.RouteUrl(nameof(GetUserImage), new { imageKey })}";
            }
            else
            {
                return $"{Request.Scheme}://{Request.Host}{Url.RouteUrl(nameof(GetUserImage), new { imageKey })}";
            }
        }
    }
}