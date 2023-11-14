using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Mcce.SmartOffice.Client.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.Client.Managers
{
    public interface IBookingManager
    {

        Task<BookingModel[]> GetList();

        Task<BookingModel[]> GetDetailList();

        Task<BookingModel> Save(BookingModel booking);

        Task ActivateBooking(string bookingNumber);

        Task<bool> CheckAvailability(string workspaceNumber, DateTime startDate, DateTime endDate);

        Task Delete(string bookingNumber);
    }

    public class BookingManager : ManagerBase<BookingModel>, IBookingManager
    {
        public BookingManager(IAppConfig appConfig, HttpClient httpClient)
            : base($"{appConfig.BaseAddress}/booking", httpClient)
        {
        }

        public async Task<BookingModel[]> GetDetailList()
        {
            var json = await HttpClient.GetStringAsync($"{BaseUrl}/details");

            var bookings = JsonConvert.DeserializeObject<BookingModel[]>(json);

            return bookings;
        }

        public async Task ActivateBooking(string bookingNumber)
        {
            await HttpClient.GetStringAsync($"{BaseUrl}/{bookingNumber}/activate");
        }

        public async Task<bool> CheckAvailability(string workspaceNumber, DateTime startDateTime, DateTime endDateTime)
        {
            var result = await HttpClient.GetFromJsonAsync<bool>($"{BaseUrl}/{workspaceNumber}/checkavailability?startDateTime={startDateTime:s}&endDateTime={endDateTime:s}");

            return result;
        }
    }
}
