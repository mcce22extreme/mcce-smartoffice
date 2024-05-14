using AutoMapper;
using Mcce.SmartOffice.Api.Entities;
using Mcce.SmartOffice.Api.Models;

namespace Mcce.SmartOffice.Api.Profiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingModel>();

            CreateMap<SaveBookingModel, Booking>();
        }
    }
}
