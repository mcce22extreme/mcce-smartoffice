namespace Mcce.SmartOffice.Core.Configs
{
    public class AuthConfig
    {
        public string AuthUrl { get; set; }

        private string _authFrontendUrl;
        public string AuthFrontendUrl
        {
            get { return _authFrontendUrl ?? AuthUrl; }
            set { _authFrontendUrl = value; }
        }

        public string ClientId { get; set; }
    }
}
