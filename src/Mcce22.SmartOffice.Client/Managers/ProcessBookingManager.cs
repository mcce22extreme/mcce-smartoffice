using System.Net.Http;
using System.Threading.Tasks;

namespace Mcce22.SmartOffice.Client.Managers
{
    public interface IProcessBookingManager
    {
        Task ProcessBookings();
    }

    public class ProcessBookingManager : IProcessBookingManager
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public ProcessBookingManager(IAppConfig appConfig, HttpClient httpClient)
        {
            _baseUrl = $"{appConfig.BaseAddress}/notify/";
            _httpClient = httpClient;
        }

        public async Task ProcessBookings()
        {
            await _httpClient.PostAsync($"{_baseUrl}", new StringContent(string.Empty));
        }
    }
}
