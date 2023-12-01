using Mcce.SmartOffice.MobileApp.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.MobileApp.Managers
{
    public interface IAccountManager
    {
        Task<AccountModel> GetAccountInfo();
    }

    public class AccountManager : ManagerBase, IAccountManager
    {
        public AccountManager(IAppConfig appConfig, IHttpClientFactory httpClientFactory, ISecureStorage secureStorage)
            : base(appConfig, httpClientFactory, secureStorage)
        {
        }

        public async Task<AccountModel> GetAccountInfo()
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}account";

            var json = await httpClient.GetStringAsync(url);

            var accountInfo = JsonConvert.DeserializeObject<AccountModel>(json);

            return accountInfo;
        }
    }
}
