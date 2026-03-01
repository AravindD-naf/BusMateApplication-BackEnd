using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.DTOs.Responses;
using BusTicketingSystem.Exceptions;
using BusTicketingSystem.Interfaces.Repositories;
using BusTicketingSystem.Models;
using BusTicketingSystem.Models.Enums;
using BusTicketingSystem.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusTicketingSystem.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<IScheduleRepository> _mockScheduleRepository;
        private readonly Mock<ISeatRepository> _mockSeatRepository;
        private readonly Mock<ISeatService> _mockSeatService;
        private readonly Mock<IAuditRepository> _mockAuditRepository;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockScheduleRepository = new Mock<IScheduleRepository>();
            _mockSeatRepository = new Mock<ISeatRepository>();
            _mockSeatService = new Mock<ISeatService>();
            _mockAuditRepository = new Mock<IAuditRepository>();

            _bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockScheduleRepository.Object,
                _mockSeatRepository.Object,
                _mockSeatService.Object,
                _mockAuditRepository.Object
            );
        }

        #region CreateBookingAsync Tests

        [Fact]
        public async Task CreateBookingAsync_WithValidInput_ShouldCreateBooking()
        {
            // Arrange
            var dto = new CreateBookingRequestDto
            {
                ScheduleId = 1,
                SeatNumbers = new List<int> { 1, 2 }
            };
            var userId = 1;
            var ipAddress = "127.0.0.1";

            var schedule = new Schedule
            {
                ScheduleId = 1,
                TravelDate = DateTime.UtcNow.AddDays(1),
                DepartureTime = TimeSpan.FromHours(10),
                AvailableSeats = 2,
                IsActive = true,
                IsDeleted = false
            };

            var seats = new List<Seat>
            {
                new Seat { SeatNumber = 1, SeatStatus = "Locked", LockedByUserId = userId },
                new Seat { SeatNumber = 2, SeatStatus = "Locked", LockedByUserId = userId }
            };

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(schedule);

            _mockSeatRepository.Setup(r => r.GetSeatsByNumbersAsync(It.IsAny<int>(), It.IsAny<List<int>>()))
                .ReturnsAsync(seats);

            _mockScheduleRepository.Setup(r => r.BeginTransactionAsync())
                .ReturnsAsync(Mock.Of<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>());

            // Act
            var result = await _bookingService.CreateBookingAsync(dto, userId, ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Once);
            _mockAuditRepository.Verify(r => r.LogAuditAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<object>(),
                It.IsAny<int>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_WithEmptySeatList_ShouldThrowValidationException()
        {
            // Arrange
            var dto = new CreateBookingRequestDto
            {
                ScheduleId = 1,
                SeatNumbers = new List<int>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _bookingService.CreateBookingAsync(dto, 1, "127.0.0.1"));
        }

        [Fact]
        public async Task CreateBookingAsync_WithInvalidSchedule_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            var dto = new CreateBookingRequestDto
            {
                ScheduleId = 999,
                SeatNumbers = new List<int> { 1 }
            };

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Schedule)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _bookingService.CreateBookingAsync(dto, 1, "127.0.0.1"));
        }

        [Fact]
        public async Task CreateBookingAsync_WithPastDeparture_ShouldThrowBookingOperationException()
        {
            // Arrange
            var dto = new CreateBookingRequestDto
            {
                ScheduleId = 1,
                SeatNumbers = new List<int> { 1 }
            };

            var schedule = new Schedule
            {
                ScheduleId = 1,
                TravelDate = DateTime.UtcNow.AddDays(-1),
                DepartureTime = TimeSpan.FromHours(10),
                IsActive = true
            };

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(schedule);

            // Act & Assert
            await Assert.ThrowsAsync<BookingOperationException>(() =>
                _bookingService.CreateBookingAsync(dto, 1, "127.0.0.1"));
        }

        [Fact]
        public async Task CreateBookingAsync_WithInsufficientSeats_ShouldThrowBookingOperationException()
        {
            // Arrange
            var dto = new CreateBookingRequestDto
            {
                ScheduleId = 1,
                SeatNumbers = new List<int> { 1, 2, 3 }
            };

            var schedule = new Schedule
            {
                ScheduleId = 1,
                TravelDate = DateTime.UtcNow.AddDays(1),
                DepartureTime = TimeSpan.FromHours(10),
                AvailableSeats = 2,
                IsActive = true
            };

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(schedule);

            // Act & Assert
            await Assert.ThrowsAsync<BookingOperationException>(() =>
                _bookingService.CreateBookingAsync(dto, 1, "127.0.0.1"));
        }

        #endregion

        #region GetBookingByIdAsync Tests

        [Fact]
        public async Task GetBookingByIdAsync_WithValidId_ShouldReturnBooking()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking
            {
                BookingId = bookingId,
                ScheduleId = 1,
                NumberOfSeats = 2,
                TotalAmount = 1000,
                IsDeleted = false
            };

            var schedule = new Schedule
            {
                ScheduleId = 1,
                Route = new Route { RouteId = 1, Source = "NYC", Destination = "Boston" },
                Bus = new Bus { BusId = 1, BusNumber = "BUS001", TotalSeats = 50 }
            };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId))
                .ReturnsAsync(booking);

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(schedule);

            // Act
            var result = await _bookingService.GetBookingByIdAsync(bookingId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetBookingByIdAsync_WithInvalidId_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Booking)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _bookingService.GetBookingByIdAsync(999));
        }

        #endregion

        #region GetMyBookingsAsync Tests

        [Fact]
        public async Task GetMyBookingsAsync_WithValidUserId_ShouldReturnBookings()
        {
            // Arrange
            var userId = 1;
            var bookings = new List<Booking>
            {
                new Booking { BookingId = 1, UserId = userId, NumberOfSeats = 2 },
                new Booking { BookingId = 2, UserId = userId, NumberOfSeats = 1 }
            };

            _mockBookingRepository.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetMyBookingsAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }

        #endregion
    }
}
