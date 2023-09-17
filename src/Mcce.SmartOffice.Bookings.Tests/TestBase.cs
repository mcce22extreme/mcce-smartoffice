using AutoMapper;
using FakeItEasy;
using Mcce.SmartOffice.Bookings.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Bookings.Tests
{
    public abstract class TestBase
    {
        protected static IMapper Mapper { get; }

        static TestBase()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(Booking).Assembly);
                cfg.AllowNullCollections = true;
            });

            Mapper = config.CreateMapper();
        }

        protected AppDbContext CreateDbContext()
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder();

            dbOptionsBuilder.UseInMemoryDatabase("smartoffice");

            return new AppDbContext(dbOptionsBuilder.Options, A.Fake<IHttpContextAccessor>());
        }
    }
}
