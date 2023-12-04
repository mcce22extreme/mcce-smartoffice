using Mcce.SmartOffice.App.Extensions;

namespace Mcce.SmartOffice.App.Managers
{
    public abstract class ManagerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISecureStorage _secureStorage;

        protected IAppConfig AppConfig { get; }

        public ManagerBase(IAppConfig appConfig, IHttpClientFactory httpClientFactory, ISecureStorage secureStorage)
        {   
            _httpClientFactory = httpClientFactory;
            _secureStorage = secureStorage;

            AppConfig = appConfig;
        }

        protected async Task<HttpClient> CreateHttpClient()
        {
            var accessToken = await _secureStorage.GetAsync(AuthConstants.ACCESS_TOKEN);

            var httpClient = _httpClientFactory.CreateClient();

            httpClient.AddAuthHeader(accessToken);

            return httpClient;
        }
    }
}
