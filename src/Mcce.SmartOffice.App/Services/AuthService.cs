using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;

namespace Mcce.SmartOffice.App.Services
{
    public interface IAuthService
    {
        Task<bool> SignIn();

        Task SignOut();

        Task<bool> RefreshAccessToken();
    }

    public class AuthService : IAuthService
    {
        private readonly OidcClient _client;
        private readonly IConnectivity _connectivity;
        private readonly ISecureStorage _secureStorage;
        private readonly IDialogService _dialogService;

        public object LoginPage { get; private set; }

        public AuthService(
            OidcClient client,
            IConnectivity connectivity,
            ISecureStorage secureStorage,
            IDialogService dialogService)
        {
            _client = client;
            _connectivity = connectivity;
            _secureStorage = secureStorage;
            _dialogService = dialogService;
        }

        public async Task<bool> SignIn()
        {
            try
            {
                if (_connectivity.NetworkAccess is not NetworkAccess.Internet)
                {
                    await _dialogService.ShowDialog("Internet Offline", "You don't appear to be connected to the internet!");
                    return false;
                }

                var loginResult = await _client.LoginAsync(new LoginRequest());
                if (loginResult.IsError)
                {
                    await _dialogService.ShowErrorMessage(loginResult.Error);
                    return false;
                }

                await _secureStorage.SetAsync(AuthConstants.IDENTITY_TOKEN, loginResult.IdentityToken);
                await _secureStorage.SetAsync(AuthConstants.ACCESS_TOKEN, loginResult.AccessToken);
                await _secureStorage.SetAsync(AuthConstants.REFRESH_TOKEN, loginResult.RefreshToken);

                return true;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorMessage(ex.ToString());
                return false;
            }
        }

        public async Task SignOut()
        {
            var identityToken = await _secureStorage.GetAsync(AuthConstants.IDENTITY_TOKEN);

            _secureStorage.Remove(AuthConstants.IDENTITY_TOKEN);
            _secureStorage.Remove(AuthConstants.ACCESS_TOKEN);
            _secureStorage.Remove(AuthConstants.REFRESH_TOKEN);

            await _client.LogoutAsync(new LogoutRequest
            {
                IdTokenHint = identityToken,
                BrowserDisplayMode = DisplayMode.Hidden
            });
        }

        public async Task<bool> RefreshAccessToken()
        {
            try
            {
                if (_connectivity.NetworkAccess is not NetworkAccess.Internet)
                {
                    await _dialogService.ShowDialog("Internet Offline", "You don't appear to be connected to the internet!");
                    return false;
                }

                var refreshToken =await _secureStorage.GetAsync(AuthConstants.REFRESH_TOKEN);

                var refreshTokenResult = await _client.RefreshTokenAsync(refreshToken);

                if (refreshTokenResult.IsError)
                {
                    return false;
                }

                await _secureStorage.SetAsync(AuthConstants.IDENTITY_TOKEN, refreshTokenResult.IdentityToken);
                await _secureStorage.SetAsync(AuthConstants.ACCESS_TOKEN, refreshTokenResult.AccessToken);
                await _secureStorage.SetAsync(AuthConstants.REFRESH_TOKEN, refreshTokenResult.RefreshToken);

                return true;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorMessage(ex.ToString());
                return false;
            }
        }
    }
}
