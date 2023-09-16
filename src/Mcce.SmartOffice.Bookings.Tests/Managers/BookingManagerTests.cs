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
            var otherUsers = await manager.GetBookings();

            // Assert
            foreach (var expectedUser in expectedBookings)
            {
                var otherUser = otherUsers.FirstOrDefault(x => x.Id == expectedUser.Id);

                Assert.IsNotNull(otherUser);
                Assert.That(otherUser.StartDateTime, Is.EqualTo(expectedUser.StartDateTime));
                Assert.That(otherUser.EndDateTime, Is.EqualTo(expectedUser.EndDateTime));
                Assert.That(otherUser.WorkspaceNumber, Is.EqualTo(expectedUser.WorkspaceNumber));
                Assert.That(otherUser.UserName, Is.EqualTo(expectedUser.UserName));
                Assert.That(otherUser.Activated, Is.False);
                Assert.That(otherUser.InvitationSent, Is.False);
            }
        }
    }
}
