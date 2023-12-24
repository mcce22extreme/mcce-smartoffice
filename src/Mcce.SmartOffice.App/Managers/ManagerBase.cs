using Mcce.SmartOffice.App.Extensions;

namespace Mcce.SmartOffice.App.Managers
{
    public abstract class ManagerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        protected IAppConfig AppConfig { get; }

        protected ISecureStorage SecureStorage { get; }

        public ManagerBase(IAppConfig appConfig, IHttpClientFactory httpClientFactory, ISecureStorage secureStorage)
        {
            _httpClientFactory = httpClientFactory;
            SecureStorage = secureStorage;
            AppConfig = appConfig;
        }

        protected async Task<HttpClient> CreateHttpClient()
        {
            var accessToken = await SecureStorage.GetAsync(AuthConstants.ACCESS_TOKEN);

            var httpClient = _httpClientFactory.CreateClient();

            httpClient.AddAuthHeader(accessToken);

            return httpClient;
        }

        //protected async Task<T> ExecuteRequest<T>(Func<HttpClient, Task<T>> func)
        //{
        //    var accessToken = await SecureStorage.GetAsync(AuthConstants.ACCESS_TOKEN);

        //    using var httpClient = await CreateHttpClient();

        //    httpClient.AddAuthHeader(accessToken);

        //    await SecureStorage.SetAsync(AuthConstants.ACCESS_TOKEN, "abc");
        //    await SecureStorage.SetAsync(AuthConstants.REFRESH_TOKEN, "abc");

        //    try
        //    {
        //        return await func(httpClient);
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        if(ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //        {
        //            if (await _authService.RefreshAccessToken())
        //            {
        //                return await ExecuteRequest(func);
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}
    }
}
