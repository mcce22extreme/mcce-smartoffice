using Mcce.SmartOffice.MobileApp.Extensions;
using Mcce.SmartOffice.MobileApp.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.MobileApp.Managers
{
    public interface IBookingManager
    {
        Task<BookingModel[]> GetMyBookings();

        Task CancelBooking(string bookingNumber);
    }

    public class BookingManager : IBookingManager
    {
        private readonly IAppConfig _appConfig;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISecureStorage _secureStorage;

        public BookingManager(IAppConfig appConfig, IHttpClientFactory httpClientFactory, ISecureStorage secureStorage)
        {
            _appConfig = appConfig;
            _httpClientFactory = httpClientFactory;
            _secureStorage = secureStorage;
        }

        public async Task<BookingModel[]> GetMyBookings()
        {
            var accessToken = await _secureStorage.GetAsync(Constants.ACCESS_TOKEN);

            using var httpClient = _httpClientFactory.CreateClient();

            httpClient.AddAuthHeader(accessToken);

            var url = $"{_appConfig.BaseAddress}booking?onlyMyBookings=true";

            var json = await httpClient.GetStringAsync(url);
            var bookings = JsonConvert.DeserializeObject<BookingModel[]>(json);

            return bookings;
        }

        public async Task CancelBooking(string bookingNumber)
        {
            var accessToken = await _secureStorage.GetAsync(Constants.ACCESS_TOKEN);

            using var httpClient = _httpClientFactory.CreateClient();

            httpClient.AddAuthHeader(accessToken);

            var url = $"{_appConfig.BaseAddress}booking/{bookingNumber}";

            await httpClient.DeleteAsync(url);
        }
    }
}
