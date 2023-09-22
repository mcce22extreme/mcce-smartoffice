using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.UserImages.Managers;
using Mcce.SmartOffice.UserImages.Models;
using Microsoft.AspNetCore.Authorization;
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
            return await _userImageManager.GetUserImages();
        }

        [AllowAnonymous]
        [HttpGet("{imageKey}", Name = nameof(GetUserImage))]
        public async Task<Stream> GetUserImage(string imageKey)
        {
            return await _userImageManager.GetUserImage(imageKey);
        }

        [HttpPost]
        public async Task<UserImageModel> StoreUserImage(IFormFile file)
        {
            return await _userImageManager.StoreUserImage(file);
        }

        [HttpDelete("{imageKey}")]
        public async Task DeleteUserImage(string imageKey)
        {
            await _userImageManager.DeleteUserImage(imageKey);
        }
    }
}
