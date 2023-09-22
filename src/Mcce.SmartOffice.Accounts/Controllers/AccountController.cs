using Mcce.SmartOffice.Accounts.Managers;
using Mcce.SmartOffice.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.Accounts.Controllers
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
