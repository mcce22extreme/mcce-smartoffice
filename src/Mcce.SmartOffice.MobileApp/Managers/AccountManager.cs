using Mcce.SmartOffice.MobileApp.Extensions;
using Mcce.SmartOffice.MobileApp.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.MobileApp.Managers
{
    public interface IAccountManager
    {
        Task<AccountModel> GetAccountInfo();
    }

    public class AccountManager : IAccountManager
    {
        private readonly IAppConfig _appConfig;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISecureStorage _secureStorage;

        public AccountManager(IAppConfig appConfig, IHttpClientFactory httpClientFactory, ISecureStorage secureStorage)
        {
            _appConfig = appConfig;
            _httpClientFactory = httpClientFactory;
            _secureStorage = secureStorage;
        }

        public async Task<AccountModel> GetAccountInfo()
        {
            var accessToken = await _secureStorage.GetAsync(Constants.ACCESS_TOKEN);

            using var httpClient = _httpClientFactory.CreateClient();

            httpClient.AddAuthHeader(accessToken);

            var url = $"{_appConfig.BaseAddress}account";

            var json = await httpClient.GetStringAsync(url);
            var accountInfo = JsonConvert.DeserializeObject<AccountModel>(json);

            return accountInfo;
        }
    }
}
