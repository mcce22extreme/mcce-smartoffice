using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Handler;
using CefSharp.Wpf;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;

namespace Mcce22.SmartOffice.Client.Services
{
    public interface IAuthService
    {
        bool LoggedIn { get; }

        Task<bool> Login();

        Task<bool> Logout();
    }

    internal class AuthService : IAuthService
    {
        private readonly IAppConfig _appConfig;
        private readonly HttpClient _httpClient;

        private string _idToken;

        public bool LoggedIn { get; private set; }

        public AuthService(IAppConfig appConfig, HttpClient httpClient)
        {
            _appConfig = appConfig;
            _httpClient = httpClient;
        }

        public async Task<bool> Login()
        {
            var client = GetClient();

            var result = await client.LoginAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            _idToken = result.IdentityToken;

            LoggedIn = !result.IsError;

            return LoggedIn;
        }

        public async Task<bool> Logout()
        {
            var client = GetClient();

            await client.LogoutAsync(new LogoutRequest
            {
                IdTokenHint = _idToken,
                BrowserDisplayMode = DisplayMode.Hidden
            });

            LoggedIn = false;

            return true;
        }

        private OidcClient GetClient()
        {
            var options = new OidcClientOptions
            {
                Authority = _appConfig.AuthEndpoint,
                ClientId = _appConfig.ClientId,
                Scope = "openid profile email roles",
                RedirectUri = "http://127.0.0.1/smartoffice",
                ClientSecret = _appConfig.ClientSecret,
                Browser = new WpfEmbeddedBrowser(),
                Policy = new Policy
                {
                    RequireIdentityTokenSignature = false
                }
            };

            return new OidcClient(options);
        }
    }

    public class WpfEmbeddedBrowser : IdentityModel.OidcClient.Browser.IBrowser
    {
        private BrowserOptions _options = null;

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            _options = options;

            var semaphoreSlim = new SemaphoreSlim(0, 1);
            var browserResult = new BrowserResult()
            {
                ResultType = BrowserResultType.UserCancel
            };

            var signinWindow = new Window()
            {
                Width = 400,
                Height = 450,
                Title = $"THE SMART OFFICE - Sign In",
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow
            };
            signinWindow.Closing += (s, e) =>
            {
                semaphoreSlim.Release();
            };

            var webView = new ChromiumWebBrowser();

            var requestHandler = new CustomRequestHandler(_options.EndUrl);
            requestHandler.BeforeNavigating += (s, e) =>
            {
                if (IsBrowserNavigatingToRedirectUri(new Uri(e.Uri)))
                {
                    signinWindow.Dispatcher.Invoke(() =>
                    {
                        e.Cancel = true;

                        browserResult = new BrowserResult()
                        {
                            ResultType = BrowserResultType.Success,
                            Response = new Uri(e.Uri).AbsoluteUri
                        };

                        semaphoreSlim.Release();
                        signinWindow.Close();
                    });
                }
            };

            webView.IsBrowserInitializedChanged += (s, a) =>
            {
                if ((bool)a.NewValue == true)
                {
                    var wv = s as ChromiumWebBrowser;
                    wv.GetCookieManager().DeleteCookiesAsync();
                }
            };

            webView.RequestHandler = requestHandler;

            signinWindow.Content = webView;
            signinWindow.Show();
            signinWindow.Focus();

            webView.Address = _options.StartUrl;

            await semaphoreSlim.WaitAsync();

            return browserResult;
        }

        private bool IsBrowserNavigatingToRedirectUri(Uri uri)
        {
            return uri.AbsoluteUri.StartsWith(_options.EndUrl);
        }
    }

    public class CustomRequestHandler : RequestHandler
    {
        private readonly string _callbackUri;

        public event EventHandler<NavigatingEventArgs> BeforeNavigating;

        public CustomRequestHandler(string callbackUri)
        {
            _callbackUri = callbackUri;
        }

        protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            var eventArgs = new NavigatingEventArgs(request.Url);

            BeforeNavigating?.Invoke(this, eventArgs);

            if (eventArgs.Cancel)
            {
                return false;
            }

            return eventArgs.Cancel ? false : base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
        }
    }

    public class NavigatingEventArgs : EventArgs
    {
        public string Uri { get; }

        public bool Cancel { get; set; }

        public NavigatingEventArgs(string url)
        {
            Uri = url;
        }
    }
}
