using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.Core.Models;

namespace Mcce.SmartOffice.Accounts.Managers
{
    public interface IAccountManager
    {
        Task<UserInfoModel> GetAccountInfo();
    }

    public class AccountManager : IAccountManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<UserInfoModel> GetAccountInfo()
        {
            var userInfo = _httpContextAccessor.GetUserInfo();

            return Task.FromResult(userInfo);
        }
    }
}
