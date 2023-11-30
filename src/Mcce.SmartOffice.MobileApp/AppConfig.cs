namespace Mcce.SmartOffice.MobileApp
{
    public interface IAppConfig
    {
        string BaseAddress { get; }

        string AuthEndpoint { get; }
    }

    public class AppConfig : IAppConfig
    {
        public string BaseAddress { get; set; }

        public string AuthEndpoint { get; set; }
    }
}
