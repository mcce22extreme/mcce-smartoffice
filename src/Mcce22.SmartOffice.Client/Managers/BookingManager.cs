using System.Net.Http;
using System.Threading.Tasks;
using Mcce22.SmartOffice.Client.Models;

namespace Mcce22.SmartOffice.Client.Managers
{
    public interface IBookingManager
    {
        Task<BookingModel[]> GetList();

        Task<BookingModel> Save(BookingModel booking);

        Task Delete(string bookingId);
    }

    public class BookingManager : ManagerBase<BookingModel>, IBookingManager
    {
        public BookingManager(IAppConfig appConfig, HttpClient httpClient)
            : base($"{appConfig.BaseAddress}/booking/", httpClient)
        {
        }
    }
}
