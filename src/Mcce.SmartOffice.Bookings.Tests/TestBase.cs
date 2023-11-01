using AutoMapper;
using FakeItEasy;
using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Core.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Bookings.Tests
{
    public abstract class TestBase
    {
        protected static IMapper Mapper { get; }

        protected AppDbContext DbContext { get; private set; }

        static TestBase()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(Booking).Assembly);
                cfg.AllowNullCollections = true;
            });

            Mapper = config.CreateMapper();
        }

        public TestBase()
        {
            //var dbOptionsBuilder = new DbContextOptionsBuilder();

            //dbOptionsBuilder.UseInMemoryDatabase("smartoffice");

            //DbContext = new AppDbContext(dbOptionsBuilder.Options, A.Fake<IHttpContextAccessor>());
        }

        //private AppDbContext CreateDbContext()
        //{
        //    var dbOptionsBuilder = new DbContextOptionsBuilder();

        //    dbOptionsBuilder.UseInMemoryDatabase("smartoffice");

        //    return new AppDbContext(dbOptionsBuilder.Options, A.Fake<IHttpContextAccessor>());
        //}

        [SetUp]
        public void Setup()
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder();

            dbOptionsBuilder.UseInMemoryDatabase("smartoffice");

            DbContext = new AppDbContext(dbOptionsBuilder.Options, A.Fake<IHttpContextAccessor>(), A.Fake<IAppConfig>());
        }

        [TearDown]
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (DbContext != null)
            {
                DbContext.Database.EnsureDeleted();
                DbContext.Dispose();
                DbContext = null;
            }
        }
    }
}
