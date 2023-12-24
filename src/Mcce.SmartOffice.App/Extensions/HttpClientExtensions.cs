using System.Net.Http.Headers;

namespace Mcce.SmartOffice.App.Extensions
{
    public static class HttpClientExtensions
    {
        public static void AddAuthHeader(this HttpClient httpClient, string accessToken)
        {
            httpClient.DefaultRequestHeaders.Remove("Authorization");

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }
    }
}
