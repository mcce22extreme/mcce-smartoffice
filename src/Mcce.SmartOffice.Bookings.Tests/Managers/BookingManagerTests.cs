using FakeItEasy;
using FluentValidation;
using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Bookings.Models;
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
                BookingNumber = Make.Identifier(),
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

            foreach (var booking in expectedBookings)
            {
                await DbContext.Bookings.AddAsync(booking);
            }

            await DbContext.SaveChangesAsync();

            // Act
            var manager = new BookingManager(Make.String(), DbContext, Mapper, A.Fake<IHttpContextAccessor>(), A.Fake<IMessageService>());
            var otherBookings = await manager.GetBookings();

            // Assert
            foreach (var expectedBooking in expectedBookings)
            {
                var otherBooking = otherBookings.FirstOrDefault(x => x.BookingNumber == expectedBooking.BookingNumber);

                Assert.IsNotNull(otherBooking);
                Assert.That(otherBooking.StartDateTime, Is.EqualTo(expectedBooking.StartDateTime));
                Assert.That(otherBooking.EndDateTime, Is.EqualTo(expectedBooking.EndDateTime));
                Assert.That(otherBooking.WorkspaceNumber, Is.EqualTo(expectedBooking.WorkspaceNumber));
                Assert.That(otherBooking.UserName, Is.EqualTo(expectedBooking.UserName));
                Assert.That(otherBooking.Activated, Is.False);
                Assert.That(otherBooking.InvitationSent, Is.False);
            }
        }

        public static TestCaseData[] CollisionTestCaseData
        {
            get
            {
                var booking = new Booking
                {
                    BookingNumber = Make.String(),
                    WorkspaceNumber = Make.String(),
                    StartDateTime = DateTime.Now,
                    EndDateTime = DateTime.Now.AddHours(1),
                };

                var testCaseData = new List<TestCaseData>
                {
                    // Same time -> collision
                    new TestCaseData(booking, new SaveBookingModel
                    {
                        WorkspaceNumber = booking.WorkspaceNumber,
                        StartDateTime = booking.StartDateTime,
                        EndDateTime = booking.EndDateTime
                    }, true),

                    // Starts before buts ends within -> collision
                    new TestCaseData(booking, new SaveBookingModel
                    {
                        WorkspaceNumber = booking.WorkspaceNumber,
                        StartDateTime = booking.StartDateTime.Subtract(TimeSpan.FromHours(1)),
                        EndDateTime = booking.StartDateTime.AddMinutes(5)
                    }, true),

                    // Starts after and ends within -> collision
                    new TestCaseData(booking, new SaveBookingModel
                    {
                        WorkspaceNumber = booking.WorkspaceNumber,
                        StartDateTime = booking.StartDateTime.AddMinutes(10),
                        EndDateTime = booking.StartDateTime.AddMinutes(20)
                    }, true),

                    // Starts within and ends after -> collision
                    new TestCaseData(booking, new SaveBookingModel
                    {
                        WorkspaceNumber = booking.WorkspaceNumber,
                        StartDateTime = booking.StartDateTime.AddMinutes(10),
                        EndDateTime = booking.EndDateTime.AddMinutes(20)
                    }, true),

                    // Starts before and ends after -> collision
                    new TestCaseData(booking, new SaveBookingModel
                    {
                        WorkspaceNumber = booking.WorkspaceNumber,
                        StartDateTime = booking.StartDateTime.Subtract(TimeSpan.FromHours(1)),
                        EndDateTime = booking.EndDateTime.AddMinutes(20)
                    }, true),

                    // Starts before and ends before -> no collision
                    new TestCaseData(booking, new SaveBookingModel
                    {
                        WorkspaceNumber = booking.WorkspaceNumber,
                        StartDateTime = booking.StartDateTime.Subtract(TimeSpan.FromHours(2)),
                        EndDateTime = booking.StartDateTime.Subtract(TimeSpan.FromHours(1))
                    }, false),

                    // Starts after and ends after -> no collision
                    new TestCaseData(booking, new SaveBookingModel
                    {
                        WorkspaceNumber = booking.WorkspaceNumber,
                        StartDateTime = booking.EndDateTime.AddHours(1),
                        EndDateTime = booking.EndDateTime.AddHours(2)
                    }, false),
                };

                return testCaseData.ToArray();
            }
        }

        [Test]
        [TestCaseSource(nameof(CollisionTestCaseData))]
        public async Task CreateBooking_ValidatesCollision(Booking existingBooking, SaveBookingModel newBooking, bool collision)
        {
            // Arrange
            await DbContext.Bookings.AddAsync(existingBooking);

            await DbContext.SaveChangesAsync();

            // Act
            var manager = new BookingManager(Make.String(), DbContext, Mapper, A.Fake<IHttpContextAccessor>(), A.Fake<IMessageService>());
                        
            if (collision)
            {
                Assert.ThrowsAsync<ValidationException>(() => manager.CreateBooking(newBooking));
            }
            else
            {
                await manager.CreateBooking(newBooking);
            }
        }
    }
}
