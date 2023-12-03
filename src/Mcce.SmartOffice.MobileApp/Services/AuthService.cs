using IdentityModel.OidcClient;
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
        private readonly IDialogService _dialogService;

        public AuthService(OidcClient client, IConnectivity connectivity, ISecureStorage secureStorage, IDialogService dialogService)
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

                await _secureStorage.SetAsync(Constants.IDENTITY_TOKEN, loginResult.IdentityToken);
                await _secureStorage.SetAsync(Constants.ACCESS_TOKEN, loginResult.AccessToken);

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
