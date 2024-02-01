using Mcce.SmartOffice.Api.Managers;
using Mcce.SmartOffice.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountManager _accountManager;

        public AccountController(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        [HttpGet]
        public async Task<UserInfoModel> GetAccountInfo()
        {
            return await _accountManager.GetAccountInfo();
        }
    }
}
