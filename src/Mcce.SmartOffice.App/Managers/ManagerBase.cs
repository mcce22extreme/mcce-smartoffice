using Mcce.SmartOffice.App.Extensions;
using Mcce.SmartOffice.App.Services;

namespace Mcce.SmartOffice.App.Managers
{
    public abstract class ManagerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISecureStorage _secureStorage;
        private readonly IAuthService _authService;

        protected IAppConfig AppConfig { get; }

        public ManagerBase(IAppConfig appConfig, IHttpClientFactory httpClientFactory, ISecureStorage secureStorage, IAuthService authService)
        {
            _httpClientFactory = httpClientFactory;
            _secureStorage = secureStorage;
            _authService = authService;
            AppConfig = appConfig;
        }

        protected async Task<HttpClient> CreateHttpClient()
        {
            var accessToken = await _secureStorage.GetAsync(AuthConstants.ACCESS_TOKEN);

            var httpClient = _httpClientFactory.CreateClient();

            httpClient.AddAuthHeader(accessToken);

            return httpClient;
        }

        protected async Task<T> ExecuteRequest<T>(Func<HttpClient, Task<T>> func)
        {
            var accessToken = await _secureStorage.GetAsync(AuthConstants.ACCESS_TOKEN);

            using var httpClient = await CreateHttpClient();

            httpClient.AddAuthHeader(accessToken);

            try
            {
                return await func(httpClient);
            }
            catch (HttpRequestException ex)
            {
                if(ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (await _authService.RefreshAccessToken())
                    {
                        return await ExecuteRequest(func);
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
