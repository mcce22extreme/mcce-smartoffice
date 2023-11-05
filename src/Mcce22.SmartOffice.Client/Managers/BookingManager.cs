using System.Net.Http;
using System.Threading.Tasks;
using Mcce22.SmartOffice.Client.Models;

namespace Mcce22.SmartOffice.Client.Managers
{
    public interface IBookingManager
    {
        Task<BookingModel[]> GetList();

        Task<BookingModel> Save(BookingModel booking);

        Task ActivateBooking(string bookingNumber);

        Task Delete(string bookingNumber);
    }

    public class BookingManager : ManagerBase<BookingModel>, IBookingManager
    {
        public BookingManager(IAppConfig appConfig, HttpClient httpClient)
            : base($"{appConfig.BaseAddress}/booking", httpClient)
        {
        }

        public async Task ActivateBooking(string bookingNumber)
        {
            await HttpClient.GetStringAsync($"{BaseUrl}/{bookingNumber}/activate");
        }
    }
}
