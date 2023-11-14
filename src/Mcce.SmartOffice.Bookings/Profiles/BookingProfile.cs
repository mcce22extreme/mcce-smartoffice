using AutoMapper;
using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Bookings.Models;

namespace Mcce.SmartOffice.Bookings.Profiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingModel>();

            CreateMap<Booking, BookingDetail>();

            CreateMap<SaveBookingModel, Booking>();
        }
    }
}
