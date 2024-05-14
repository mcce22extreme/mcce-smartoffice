using Mcce.SmartOffice.Api.Managers;
using Mcce.SmartOffice.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.Api.Controllers
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
        public async Task<UserImageModel> StoreUserImage()
        {
            return await _userImageManager.StoreUserImage(Request.Body, Request.Headers.ContentType);
        }

        [HttpDelete("{imageKey}")]
        public async Task DeleteUserImage(string imageKey)
        {
            await _userImageManager.DeleteUserImage(imageKey);
        }
    }
}
