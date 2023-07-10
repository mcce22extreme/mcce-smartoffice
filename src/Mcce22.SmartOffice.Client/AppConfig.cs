namespace Mcce22.SmartOffice.Client
{
    public interface IAppConfig
    {
        string BaseAddress { get; }

        string AuthEndpoint { get; }

        string ClientSecret { get; }
    }

    internal class AppConfig : IAppConfig
    {
        public string BaseAddress { get; set; }

        public string AuthEndpoint { get; set; }

        public string ClientSecret { get; set; }
    }
}
