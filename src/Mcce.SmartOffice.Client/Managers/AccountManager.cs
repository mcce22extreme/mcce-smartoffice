using System.Net.Http;
using System.Threading.Tasks;
using Mcce.SmartOffice.Client.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.Client.Managers
{
    public interface IAccountManager
    {
        Task<UserModel> GetAccountInfo();
    }

    public class AccountManager : IAccountManager
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public AccountManager(IAppConfig appConfig, HttpClient httpClient)
        {
            _baseUrl = $"{appConfig.BaseAddress}/account";
            _httpClient = httpClient;
        }

        public async Task<UserModel> GetAccountInfo()
        {
            var json = await _httpClient.GetStringAsync(_baseUrl);

            return JsonConvert.DeserializeObject<UserModel>(json);
        }
    }
}
