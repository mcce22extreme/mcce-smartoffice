﻿using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;

namespace Mcce.SmartOffice.MobileApp.Services
{
    public interface IAuthService
    {
        Task<bool> SignIn();

        Task SignOut();
    }

    public class AuthService : IAuthService
    {
        private readonly OidcClient _client;
        private readonly IConnectivity _connectivity;
        private readonly ISecureStorage _secureStorage;

        public AuthService(OidcClient client, IConnectivity connectivity, ISecureStorage secureStorage)
        {
            _client = client;
            _connectivity = connectivity;
            _secureStorage = secureStorage;
        }

        public async Task<bool> SignIn()
        {
            try
            {
                if (_connectivity.NetworkAccess is not NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlert("Internet Offline", "Check sua internet e tente novamente!", "Ok");
                    return false;
                }

                var loginResult = await _client.LoginAsync(new LoginRequest());
                if (loginResult.IsError)
                {
                    await Shell.Current.DisplayAlert("Error", "An unexpected error occurred during the login process!", "OK");
                    return false;
                }

                await _secureStorage.SetAsync(Constants.IDENTITY_TOKEN, loginResult.IdentityToken);
                await _secureStorage.SetAsync(Constants.ACCESS_TOKEN, loginResult.AccessToken);

                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "ok");
                return false;
            }
        }

        public async Task SignOut()
        {
            var identityToken = await _secureStorage.GetAsync(Constants.IDENTITY_TOKEN);

            _secureStorage.Remove(Constants.IDENTITY_TOKEN);
            _secureStorage.Remove(Constants.ACCESS_TOKEN);

            await _client.LogoutAsync(new LogoutRequest
            {
                IdTokenHint = identityToken,
                BrowserDisplayMode = DisplayMode.Hidden
            });
        }
    }
}