using Mcce22.SmartOffice.MobileApp.Constants;
using Mcce22.SmartOffice.MobileApp.Extensions;
using Mcce22.SmartOffice.MobileApp.Models;
using Newtonsoft.Json;

namespace Mcce22.SmartOffice.MobileApp.Managers
{
    public interface IAccountManager
    {
        Task<UserInfoModel> GetUserInfo();
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

        public async Task<UserInfoModel> GetUserInfo()
        {
            var accessToken = await _secureStorage.GetAsync(AuthConstants.ACCESS_TOKEN);

            using var httpClient = _httpClientFactory.CreateClient();
           
            httpClient.AddAuthHeader(accessToken);

            var url = $"{_appConfig.BaseAddress}account";

            var json = await httpClient.GetStringAsync(url);
            var userInfo = JsonConvert.DeserializeObject<UserInfoModel>(json);

            return userInfo;

        }
    }
}
