namespace Mcce.SmartOffice.Client
{
    public interface IAppConfig
    {
        string BaseAddress { get; }

        string AuthEndpoint { get; }

        string ClientSecret { get; }

        string ClientId { get; }
    }

    internal class AppConfig : IAppConfig
    {
        public string BaseAddress { get; set; }

        public string AuthEndpoint { get; set; }

        public string ClientSecret { get; set; }

        public string ClientId { get; set; }
    }
}
