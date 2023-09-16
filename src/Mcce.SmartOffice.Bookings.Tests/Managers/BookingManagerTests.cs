using FakeItEasy;
using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Bookings.Services;
using Mcce.SmartOffice.Core.Services;
using Microsoft.AspNetCore.Http;

namespace Mcce.SmartOffice.Bookings.Tests.Managers
{
    [TestFixture]
    public class BookingManagerTests : TestBase
    {
        private Booking CreateBooking()
        {
            return new Booking
            {
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                WorkspaceNumber = Make.String(),
                UserName = Make.String()
            };
        }

        private List<Booking> CreateBookings()
        {
            var users = new List<Booking>();

            for (int i = 0; i < 10; i++)
            {
                users.Add(CreateBooking());
            }

            return users;
        }

        [Test]
        public async Task GetBookings_ReturnsBookings()
        {
            // Arrange
            var expectedBookings = CreateBookings();
            using var dbContext = CreateDbContext();

            foreach (var booking in expectedBookings)
            {
                await dbContext.Bookings.AddAsync(booking);
            }

            await dbContext.SaveChangesAsync();

            // Act
            var manager = new BookingManager(dbContext, Mapper, A.Fake<IEmailService>(), A.Fake<IHttpContextAccessor>(), A.Fake<IMessageService>());
            var otherBookings = await manager.GetBookings();

            // Assert
            foreach (var expectedBooking in expectedBookings)
            {
                var otherBooking = otherBookings.FirstOrDefault(x => x.Id == expectedBooking.Id);

                Assert.IsNotNull(otherBooking);
                Assert.That(otherBooking.StartDateTime, Is.EqualTo(expectedBooking.StartDateTime));
                Assert.That(otherBooking.EndDateTime, Is.EqualTo(expectedBooking.EndDateTime));
                Assert.That(otherBooking.WorkspaceNumber, Is.EqualTo(expectedBooking.WorkspaceNumber));
                Assert.That(otherBooking.UserName, Is.EqualTo(expectedBooking.UserName));
                Assert.That(otherBooking.Activated, Is.False);
                Assert.That(otherBooking.InvitationSent, Is.False);
            }
        }
    }
}
