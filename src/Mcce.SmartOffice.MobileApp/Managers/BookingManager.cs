using System.Net.Http.Json;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.App.Models;
using Mcce.SmartOffice.App.Services;
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
        public BookingManager(
            IAppConfig appConfig,
            IHttpClientFactory httpClientFactory,
            ISecureStorage secureStorage,
            IAuthService authService)
            : base(appConfig, httpClientFactory, secureStorage, authService)
        {
        }

        public Task<BookingModel[]> GetMyBookings()
        {
            return ExecuteRequest(async httpClient =>
            {

                var url = $"{AppConfig.BaseAddress}booking?onlyMyBookings=true";

                var json = await httpClient.GetStringAsync(url);

                var bookings = JsonConvert.DeserializeObject<BookingModel[]>(json);

                return bookings;
            });
        }

        public Task CreateBooking(string workspaceNumber, DateTime startDateTime, DateTime endDateTime)
        {
            return ExecuteRequest(async httpClient =>
            {
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

                return Task.CompletedTask;
            });
        }

        public Task ActivateBooking(string bookingNumber)
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}booking/{bookingNumber}/activate";

                var response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                return Task.CompletedTask;
            });
        }

        public Task CancelBooking(string bookingNumber)
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}booking/{bookingNumber}";

                var response =  await httpClient.DeleteAsync(url);

                response.EnsureSuccessStatusCode();

                return Task.CompletedTask;
            });
        }
    }
}
