using FakeItEasy;
using FluentValidation;
using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Bookings.Enums;
using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Bookings.Messages;
using Mcce.SmartOffice.Bookings.Models;
using Mcce.SmartOffice.Core.Accessors;
using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Core.Exceptions;
using Mcce.SmartOffice.Core.Models;
using Mcce.SmartOffice.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Bookings.Tests.Managers
{
    [TestFixture]
    public class BookingManagerTests : TestBase
    {
        private Booking CreateBooking(UserInfoModel user = null, BookingState state = BookingState.Pending)
        {
            return new Booking
            {
                BookingNumber = Make.Identifier(),
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                WorkspaceNumber = Make.String(),
                UserName = user?.UserName ?? Make.String(),
                FirstName = user?.FirstName ?? Make.String(),
                LastName = user?.LastName ?? Make.String(),
                State = state
            };
        }

        private List<Booking> CreateBookings(UserInfoModel user = null)
        {
            var users = new List<Booking>();

            for (int i = 0; i < 10; i++)
            {
                users.Add(CreateBooking(user));
            }

            return users;
        }

        private UserInfoModel CreateUserInfo(string userName = null, bool isAdmin = false)
        {
            return new UserInfoModel
            {
                UserName = userName ?? Make.String(),
                FirstName = Make.String(),
                LastName = Make.String(),
                IsAdmin = isAdmin
            };
        }

        [Test]
        public async Task GetList_WithOnlyMyBookings_ReturnsOnlyBookingsOfCurrentUser()
        {
            // Arrange
            var userA = CreateUserInfo(Make.String());
            var userB = CreateUserInfo(Make.String());

            var expectedBookings = CreateBookings(userA);
            expectedBookings.AddRange(CreateBookings(userB));

            foreach (var booking in expectedBookings)
            {
                await DbContext.Bookings.AddAsync(booking);
            }

            await DbContext.SaveChangesAsync();

            // Act
            var auchAccessor = A.Fake<IAuthContextAccessor>();
            A.CallTo(() => auchAccessor.GetUserInfo()).Returns(userB);
            var manager = new BookingManager(Make.String(), DbContext, Mapper, auchAccessor, A.Fake<IMessageService>());
            var otherBookings = await manager.GetBookings(onlyMyBookings: true);

            // Assert
            foreach (var expectedBooking in expectedBookings.Where(x => x.UserName == userB.UserName))
            {
                var otherBooking = otherBookings.FirstOrDefault(x => x.BookingNumber == expectedBooking.BookingNumber);

                Assert.IsNotNull(otherBooking);
                Assert.That(otherBooking.StartDateTime, Is.EqualTo(expectedBooking.StartDateTime));
                Assert.That(otherBooking.EndDateTime, Is.EqualTo(expectedBooking.EndDateTime));
                Assert.That(otherBooking.WorkspaceNumber, Is.EqualTo(expectedBooking.WorkspaceNumber));
                Assert.That(otherBooking.State, Is.EqualTo(expectedBooking.State));
                Assert.That(otherBooking.UserName, Is.EqualTo(userB.UserName));
                Assert.That(otherBooking.FirstName, Is.EqualTo(userB.FirstName));
                Assert.That(otherBooking.LastName, Is.EqualTo(userB.LastName));
            }
        }

        [Test]
        public async Task GetAllBookings_WithNonAdmin_ReturnsBookingsOfAllUsers_WithoutUserInformation()
        {
            var users = new List<UserInfoModel>();
            var expectedBookings = new List<Booking>();

            // Arrange
            for (int i = 0; i < 3; i++)
            {
                var user = CreateUserInfo(Make.String());
                var bookings = CreateBookings(user);

                users.Add(user);
                expectedBookings.AddRange(bookings);

            }

            foreach (var booking in expectedBookings)
            {
                await DbContext.Bookings.AddAsync(booking);
            }

            await DbContext.SaveChangesAsync();

            // Act
            var manager = new BookingManager(Make.String(), DbContext, Mapper, A.Fake<IAuthContextAccessor>(), A.Fake<IMessageService>());
            var otherBookings = await manager.GetBookings();

            // Assert
            foreach (var expectedBooking in expectedBookings)
            {
                var otherBooking = otherBookings.FirstOrDefault(x => x.BookingNumber == expectedBooking.BookingNumber);
                var otherUser = users.FirstOrDefault(x => x.UserName == expectedBooking.UserName);

                Assert.IsNotNull(otherBooking);
                Assert.IsNotNull(otherUser);
                Assert.That(otherBooking.StartDateTime, Is.EqualTo(expectedBooking.StartDateTime));
                Assert.That(otherBooking.EndDateTime, Is.EqualTo(expectedBooking.EndDateTime));
                Assert.That(otherBooking.WorkspaceNumber, Is.EqualTo(expectedBooking.WorkspaceNumber));
                Assert.That(otherBooking.State, Is.EqualTo(expectedBooking.State));
                Assert.IsNull(otherBooking.UserName);
                Assert.IsNull(otherBooking.FirstName);
                Assert.IsNull(otherBooking.LastName);
            }
        }

        [Test]
        public async Task GetBooking_WithNormalUsers_ReturnsBookingOfUser()
        {
            // Arrange
            var user = CreateUserInfo(Make.String());
            var expectedBooking = CreateBooking(user);
            await DbContext.Bookings.AddAsync(expectedBooking);

            await DbContext.SaveChangesAsync();

            // Act
            var auchAccessor = A.Fake<IAuthContextAccessor>();
            A.CallTo(() => auchAccessor.GetUserInfo()).Returns(user);
            var manager = new BookingManager(Make.String(), DbContext, Mapper, auchAccessor, A.Fake<IMessageService>());
            var otherBooking = await manager.GetBooking(expectedBooking.BookingNumber);

            Assert.IsNotNull(otherBooking);
            Assert.That(otherBooking.StartDateTime, Is.EqualTo(expectedBooking.StartDateTime));
            Assert.That(otherBooking.EndDateTime, Is.EqualTo(expectedBooking.EndDateTime));
            Assert.That(otherBooking.WorkspaceNumber, Is.EqualTo(expectedBooking.WorkspaceNumber));
            Assert.That(otherBooking.State, Is.EqualTo(expectedBooking.State));
            Assert.That(otherBooking.UserName, Is.EqualTo(user.UserName));
            Assert.That(otherBooking.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(otherBooking.LastName, Is.EqualTo(user.LastName));
        }

        [Test]
        public async Task GetBooking_WithNonAdminUser_WhenRetrivingBookingOfOtherUser_ThrowsException()
        {
            // Arrange
            var userA = CreateUserInfo(Make.String());
            var userB = CreateUserInfo(Make.String());
            var expectedBooking = CreateBooking(userA);
            await DbContext.Bookings.AddAsync(expectedBooking);

            await DbContext.SaveChangesAsync();

            // Act
            var auchAccessor = A.Fake<IAuthContextAccessor>();
            A.CallTo(() => auchAccessor.GetUserInfo()).Returns(userB);
            var manager = new BookingManager(Make.String(), DbContext, Mapper, auchAccessor, A.Fake<IMessageService>());

            Assert.ThrowsAsync<ForbiddenException>(() => manager.GetBooking(expectedBooking.BookingNumber));
        }

        [Test]
        public async Task GetBooking_WithAdminUser_ForBookingOfOtherUser_ReturnsBooking()
        {
            // Arrange
            var userA = CreateUserInfo(Make.String());
            var userB = CreateUserInfo(Make.String(), true);
            var expectedBooking = CreateBooking(userA);
            await DbContext.Bookings.AddAsync(expectedBooking);

            await DbContext.SaveChangesAsync();

            // Act
            var auchAccessor = A.Fake<IAuthContextAccessor>();
            A.CallTo(() => auchAccessor.GetUserInfo()).Returns(userB);
            var manager = new BookingManager(Make.String(), DbContext, Mapper, auchAccessor, A.Fake<IMessageService>());
            var otherBooking = await manager.GetBooking(expectedBooking.BookingNumber);

            Assert.IsNotNull(otherBooking);
            Assert.That(otherBooking.StartDateTime, Is.EqualTo(expectedBooking.StartDateTime));
            Assert.That(otherBooking.EndDateTime, Is.EqualTo(expectedBooking.EndDateTime));
            Assert.That(otherBooking.WorkspaceNumber, Is.EqualTo(expectedBooking.WorkspaceNumber));
            Assert.That(otherBooking.State, Is.EqualTo(expectedBooking.State));
            Assert.That(otherBooking.UserName, Is.EqualTo(userA.UserName));
            Assert.That(otherBooking.FirstName, Is.EqualTo(userA.FirstName));
            Assert.That(otherBooking.LastName, Is.EqualTo(userA.LastName));
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
                    UserName = Make.String(),
                    FirstName = Make.String(),
                    LastName = Make.String(),
                };

                var testCaseData = new List<TestCaseData>
                {
                    // Same time -> collision
                    new TestCaseData(booking, new SaveBookingModel
                    {
                        WorkspaceNumber = booking.WorkspaceNumber,
                        StartDateTime = booking.StartDateTime,
                        EndDateTime = booking.EndDateTime,
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
            var user = CreateUserInfo(Make.String());
            await DbContext.Bookings.AddAsync(existingBooking);

            await DbContext.SaveChangesAsync();

            // Act
            var auchAccessor = A.Fake<IAuthContextAccessor>();
            A.CallTo(() => auchAccessor.GetUserInfo()).Returns(user);
            var manager = new BookingManager(Make.String(), DbContext, Mapper, auchAccessor, A.Fake<IMessageService>());

            if (collision)
            {
                Assert.ThrowsAsync<ValidationException>(() => manager.CreateBooking(newBooking));
            }
            else
            {
                await manager.CreateBooking(newBooking);
            }
        }

        [Test]
        public async Task DeleteBooking_WithNonAdminUser_ForBookingOfUser_DeletesBooking()
        {
            // Arrange
            var user = CreateUserInfo(Make.String());
            var expectedBooking = CreateBooking(user);
            await DbContext.Bookings.AddAsync(expectedBooking);

            await DbContext.SaveChangesAsync();

            // Act
            var auchAccessor = A.Fake<IAuthContextAccessor>();
            A.CallTo(() => auchAccessor.GetUserInfo()).Returns(user);
            var manager = new BookingManager(Make.String(), DbContext, Mapper, auchAccessor, A.Fake<IMessageService>());
            await manager.DeleteBooking(expectedBooking.BookingNumber);

            //Assert
            var otherBooking = await DbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == expectedBooking.BookingNumber);
            Assert.IsNull(otherBooking);
        }

        [Test]
        public async Task DeleteBooking_WithNonAdminUser_ForBookingOfOtherUser_ThrowsException()
        {
            // Arrange
            var userA = CreateUserInfo(Make.String());
            var userB = CreateUserInfo(Make.String());
            var expectedBooking = CreateBooking(userA);
            await DbContext.Bookings.AddAsync(expectedBooking);

            await DbContext.SaveChangesAsync();

            // Act
            var auchAccessor = A.Fake<IAuthContextAccessor>();
            A.CallTo(() => auchAccessor.GetUserInfo()).Returns(userB);
            var manager = new BookingManager(Make.String(), DbContext, Mapper, auchAccessor, A.Fake<IMessageService>());

            // Assert
            Assert.ThrowsAsync<ForbiddenException>(() => manager.DeleteBooking(expectedBooking.BookingNumber));
        }

        [Test]
        public async Task DeleteBooking_WithAdminUser_ForBookingOfOtherUsers_DeletesBooking()
        {
            // Arrange
            var userA = CreateUserInfo(Make.String());
            var userB = CreateUserInfo(Make.String(), true);
            var expectedBooking = CreateBooking(userA);
            await DbContext.Bookings.AddAsync(expectedBooking);

            await DbContext.SaveChangesAsync();

            // Act
            var auchAccessor = A.Fake<IAuthContextAccessor>();
            A.CallTo(() => auchAccessor.GetUserInfo()).Returns(userB);
            var manager = new BookingManager(Make.String(), DbContext, Mapper, auchAccessor, A.Fake<IMessageService>());
            await manager.DeleteBooking(expectedBooking.BookingNumber);

            // Assert
            var otherBooking = await DbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == expectedBooking.BookingNumber);
            Assert.IsNull(otherBooking);
        }

        [Test]
        public async Task ActivateBooking_UpdatesState()
        {
            // Arrange
            var expectedBooking = CreateBooking(state: BookingState.Confirmed);
            await DbContext.Bookings.AddAsync(expectedBooking);
            await DbContext.SaveChangesAsync();

            // Act
            var manager = new BookingManager(Make.String(), DbContext, Mapper, A.Fake<IAuthContextAccessor>(), A.Fake<IMessageService>());
            await manager.ActivateBooking(expectedBooking.BookingNumber);

            // Assert
            var otherBooking = await DbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == expectedBooking.BookingNumber);
            Assert.That(otherBooking.State, Is.EqualTo(BookingState.Activated));
        }

        public static TestCaseData[] ActivateBookingTestCases
        {
            get
            {
                var testCaseData = new List<TestCaseData>
                {
                    // pending -> activated: invalid
                    new TestCaseData(BookingState.Pending, false),
                    // confirmed -> activated: valid
                    new TestCaseData(BookingState.Activated, true),
                    // activated -> activated: invalid
                    new TestCaseData(BookingState.Confirmed, true),
                    // rejected -> activated: invalid
                    new TestCaseData(BookingState.Rejected, false),
                };

                return testCaseData.ToArray();
            }
        }

        [Test]
        [TestCaseSource(nameof(ActivateBookingTestCases))]
        public async Task ActivateBooking_WithValidState_UpdatesState(BookingState state, bool valid)
        {
            // Arrange
            var expectedBooking = CreateBooking(state: state);
            await DbContext.Bookings.AddAsync(expectedBooking);
            await DbContext.SaveChangesAsync();

            // Act
            var manager = new BookingManager(Make.String(), DbContext, Mapper, A.Fake<IAuthContextAccessor>(), A.Fake<IMessageService>());

            if (valid)
            {
                await manager.ActivateBooking(expectedBooking.BookingNumber);

                // Assert
                var otherBooking = await DbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == expectedBooking.BookingNumber);
                Assert.That(otherBooking.State, Is.EqualTo(BookingState.Activated));
            }
            else
            {
                // Assert
                Assert.ThrowsAsync<ValidationException>(() => manager.ActivateBooking(expectedBooking.BookingNumber));
            }
        }

        [Test]
        public async Task ActivateBooking_PublishesMessage()
        {
            // Arrange
            var user = CreateUserInfo(Make.String());
            var expectedBooking = CreateBooking(user, BookingState.Confirmed);
            await DbContext.Bookings.AddAsync(expectedBooking);
            await DbContext.SaveChangesAsync();

            // Act
            var auchAccessor = A.Fake<IAuthContextAccessor>();
            A.CallTo(() => auchAccessor.GetUserInfo()).Returns(user);
            var messagService = A.Fake<IMessageService>();
            var manager = new BookingManager(Make.String(), DbContext, Mapper, auchAccessor, messagService);
            await manager.ActivateBooking(expectedBooking.BookingNumber);

            // Assert
            A.CallTo(() => messagService.Publish(
                MessageTopics.TOPIC_BOOKING_ACTIVATED.Replace("{0}", user.UserName),
                A<BookingActivatedMessage>.That.Matches(msg => msg.BookingNumber == expectedBooking.BookingNumber && msg.WorkspaceNumber == expectedBooking.WorkspaceNumber && msg.UserName == user.UserName)))
                .MustHaveHappened();
        }
    }
}
