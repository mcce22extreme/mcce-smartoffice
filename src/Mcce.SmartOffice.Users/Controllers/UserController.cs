using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Users.Managers;
using Mcce.SmartOffice.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcce22.SmartOffice.Users.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(AuthConstants.APP_ROLE_USERS)]
        public async Task<UserModel[]> GetUsers()
        {
            return await _userManager.GetUsers();
        }

        [HttpGet("{userId}")]
        [Authorize(AuthConstants.APP_ROLE_USERS)]
        public async Task<UserModel> GetUser(int userId)
        {
            return await _userManager.GetUser(userId);
        }

        [HttpDelete("{userId}")]
        [Authorize(AuthConstants.APP_ROLE_ADMINS)]
        public async Task DeleteUser(int userId)
        {
            await _userManager.DeleteUser(userId);
        }
    }
}
