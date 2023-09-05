using AutoMapper;
using FakeItEasy;
using Mcce.SmartOffice.Bookings.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Bookings.Tests
{
    public abstract class TestBase
    {
        private const string DBNAME = "Data Source=test.db";

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

        [SetUp]
        public void SetUp()
        {
            DeleteTestDb();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTestDb();
        }

        private void DeleteTestDb()
        {
            if (File.Exists(DBNAME))
            {
                File.Delete(DBNAME);
            }
        }

        protected AppDbContext CreateDbContext()
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder();
            dbOptionsBuilder.UseSqlite(DBNAME);

            var dbContext = new AppDbContext(dbOptionsBuilder.Options, A.Fake<IHttpContextAccessor>());

            if (!File.Exists(DBNAME))
            {
                dbContext.Database.Migrate();
            }

            return dbContext;
        }
    }
}
