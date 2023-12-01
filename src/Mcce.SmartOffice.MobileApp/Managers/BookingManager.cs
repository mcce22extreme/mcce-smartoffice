using System.Net.Http.Json;
using Mcce.SmartOffice.MobileApp.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.MobileApp.Managers
{
    public interface IBookingManager
    {
        Task<BookingModel[]> GetMyBookings();

        Task CreateBooking(string workspaceNumber, DateTime startDateTime, DateTime endDateTime);

        Task ActivateBooking(string bookingNumber);

        Task CancelBooking(string bookingNumber);
    }

    public class BookingManager : ManagerBase, IBookingManager
    {
        public BookingManager(IAppConfig appConfig, IHttpClientFactory httpClientFactory, ISecureStorage secureStorage)
            : base(appConfig, httpClientFactory, secureStorage)
        {
        }

        public async Task<BookingModel[]> GetMyBookings()
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}booking?onlyMyBookings=true";

            var json = await httpClient.GetStringAsync(url);

            var bookings = JsonConvert.DeserializeObject<BookingModel[]>(json);

            return bookings;
        }

        public async Task CreateBooking(string workspaceNumber, DateTime startDateTime, DateTime endDateTime)
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}booking";

            var response = await httpClient.PostAsJsonAsync(url, new
            {
                WorkspaceNumber = workspaceNumber,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime
            });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorModel>();
                if (error != null)
                {
                    throw new Exception(error.ErrorMessage);
                }
            }
        }

        public async Task ActivateBooking(string bookingNumber)
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}booking/{bookingNumber}/activate";

            await httpClient.GetAsync(url);
        }

        public async Task CancelBooking(string bookingNumber)
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}booking/{bookingNumber}";

            await httpClient.DeleteAsync(url);
        }
    }
}
