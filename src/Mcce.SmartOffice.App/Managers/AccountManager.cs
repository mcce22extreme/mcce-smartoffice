using Mcce.SmartOffice.App.Models;
using Mcce.SmartOffice.App.Services;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.App.Managers
{
    public interface IAccountManager
    {
        Task<AccountModel> GetAccountInfo();
    }

    public class AccountManager : ManagerBase, IAccountManager
    {
        public AccountManager(
            IAppConfig appConfig,
            IHttpClientFactory httpClientFactory,
            ISecureStorage secureStorage,
            IAuthService authService)
            : base(appConfig, httpClientFactory, secureStorage, authService)
        {
        }

        public Task<AccountModel> GetAccountInfo()
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}account";

                var json = await httpClient.GetStringAsync(url);

                var accountInfo = JsonConvert.DeserializeObject<AccountModel>(json);

                return accountInfo;
            });            
        }
    }
}
