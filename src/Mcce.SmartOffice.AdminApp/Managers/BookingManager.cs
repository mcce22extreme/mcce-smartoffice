using Mcce.SmartOffice.AdminApp.Models;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.AdminApp.Managers
{
    public interface IBookingManager
    {
        Task<BookingModel[]> GetBookings();

        Task CancelBooking(string bookingNumber);
    }

    public class BookingManager : ManagerBase, IBookingManager
    {
        public BookingManager(
            IAppConfig appConfig,
            IHttpClientFactory httpClientFactory,
            ISecureStorage secureStorage)
            : base(appConfig, httpClientFactory, secureStorage)
        {
        }

        public async Task<BookingModel[]> GetBookings()
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}booking";

            var json = await httpClient.GetStringAsync(url);

            var bookings = JsonConvert.DeserializeObject<BookingModel[]>(json);

            return bookings;
        }

        public async Task CancelBooking(string bookingNumber)
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}booking/{bookingNumber}";

            var response =  await httpClient.DeleteAsync(url);

            response.EnsureSuccessStatusCode();
        }
    }
}
