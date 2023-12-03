namespace Mcce.SmartOffice.App
{
    public interface IAppConfig
    {
        string BaseAddress { get; }

        string AuthEndpoint { get; }

        string AuthRedirectUri { get; }
    }

    public class AppConfig : IAppConfig
    {
        public string BaseAddress { get; set; }

        public string AuthEndpoint { get; set; }

        public string AuthRedirectUri { get; set; }
    }
}
