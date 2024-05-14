using AutoMapper;
using FakeItEasy;
using Mcce.SmartOffice.Api.Accessors;
using Mcce.SmartOffice.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Api
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

        [SetUp]
        public void Setup()
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder();

            dbOptionsBuilder.UseInMemoryDatabase("smartoffice");

            DbContext = new AppDbContext(dbOptionsBuilder.Options, A.Fake<IAuthContextAccessor>());
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
