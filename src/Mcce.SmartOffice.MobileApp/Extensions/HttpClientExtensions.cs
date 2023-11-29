﻿namespace Mcce.SmartOffice.MobileApp.Extensions
{
    public static class HttpClientExtensions
    {
        public static void AddAuthHeader(this HttpClient httpClient, string accessToken)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }
    }
}