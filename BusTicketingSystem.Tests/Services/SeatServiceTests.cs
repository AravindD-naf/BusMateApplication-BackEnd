using BusTicketingSystem.Exceptions;
using BusTicketingSystem.Interfaces.Repositories;
using BusTicketingSystem.Models;
using BusTicketingSystem.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusTicketingSystem.Tests.Services
{
    public class SeatServiceTests
    {
        private readonly Mock<ISeatRepository> _mockSeatRepository;
        private readonly Mock<ISeatLockRepository> _mockSeatLockRepository;
        private readonly Mock<IScheduleRepository> _mockScheduleRepository;
        private readonly Mock<IAuditRepository> _mockAuditRepository;
        private readonly SeatService _seatService;

        public SeatServiceTests()
        {
            _mockSeatRepository = new Mock<ISeatRepository>();
            _mockSeatLockRepository = new Mock<ISeatLockRepository>();
            _mockScheduleRepository = new Mock<IScheduleRepository>();
            _mockAuditRepository = new Mock<IAuditRepository>();

            _seatService = new SeatService(
                _mockSeatRepository.Object,
                _mockSeatLockRepository.Object,
                _mockScheduleRepository.Object,
                _mockAuditRepository.Object
            );
        }

        #region GetSeatLayoutAsync Tests

        [Fact]
        public async Task GetSeatLayoutAsync_WithValidSchedule_ShouldReturnSeatLayout()
        {
            // Arrange
            var scheduleId = 1;
            var schedule = new Schedule
            {
                ScheduleId = scheduleId,
                BusId = 1,
                TotalSeats = 50,
                AvailableSeats = 40,
                IsActive = true,
                IsDeleted = false,
                Bus = new Bus { BusNumber = "BUS001" }
            };

            var seats = new List<Seat>
            {
                new Seat { SeatId = 1, SeatNumber = 1, SeatStatus = "Available" },
                new Seat { SeatId = 2, SeatNumber = 2, SeatStatus = "Locked" },
                new Seat { SeatId = 3, SeatNumber = 3, SeatStatus = "Booked" }
            };

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _mockSeatRepository.Setup(r => r.GetSeatsByScheduleIdAsync(scheduleId))
                .ReturnsAsync(seats);

            // Act
            var result = await _seatService.GetSeatLayoutAsync(scheduleId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.TotalSeats.Should().Be(50);
            result.Data.AvailableSeats.Should().Be(40);
            result.Data.Seats.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetSeatLayoutAsync_WithInvalidSchedule_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            _mockScheduleRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Schedule)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _seatService.GetSeatLayoutAsync(999));
        }

        [Fact]
        public async Task GetSeatLayoutAsync_WithDeletedSchedule_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            var schedule = new Schedule
            {
                ScheduleId = 1,
                IsDeleted = true
            };

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(schedule);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _seatService.GetSeatLayoutAsync(1));
        }

        #endregion

        #region LockSeatsAsync Tests

        [Fact]
        public async Task LockSeatsAsync_WithAvailableSeats_ShouldLockSeats()
        {
            // Arrange
            var scheduleId = 1;
            var seatNumbers = new List<int> { 1, 2 };
            var userId = 1;

            var schedule = new Schedule
            {
                ScheduleId = scheduleId,
                IsActive = true,
                IsDeleted = false
            };

            var seats = new List<Seat>
            {
                new Seat { SeatId = 1, SeatNumber = 1, SeatStatus = "Available" },
                new Seat { SeatId = 2, SeatNumber = 2, SeatStatus = "Available" }
            };

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _mockSeatRepository.Setup(r => r.GetSeatsByNumbersAsync(scheduleId, seatNumbers))
                .ReturnsAsync(seats);

            // Act
            var result = await _seatService.LockSeatsAsync(scheduleId, seatNumbers, userId, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task LockSeatsAsync_WithLockedSeat_ShouldThrowSeatOperationException()
        {
            // Arrange
            var scheduleId = 1;
            var seatNumbers = new List<int> { 1 };
            var userId = 1;

            var schedule = new Schedule
            {
                ScheduleId = scheduleId,
                IsActive = true,
                IsDeleted = false
            };

            var seats = new List<Seat>
            {
                new Seat { SeatId = 1, SeatNumber = 1, SeatStatus = "Locked", LockedByUserId = 2 }
            };

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _mockSeatRepository.Setup(r => r.GetSeatsByNumbersAsync(scheduleId, seatNumbers))
                .ReturnsAsync(seats);

            // Act & Assert
            await Assert.ThrowsAsync<SeatOperationException>(() =>
                _seatService.LockSeatsAsync(scheduleId, seatNumbers, userId, "127.0.0.1"));
        }

        #endregion

        #region UnlockSeatsAsync Tests

        [Fact]
        public async Task UnlockSeatsAsync_WithLockedSeats_ShouldUnlockSeats()
        {
            // Arrange
            var scheduleId = 1;
            var seatNumbers = new List<int> { 1, 2 };
            var userId = 1;

            var seats = new List<Seat>
            {
                new Seat { SeatId = 1, SeatNumber = 1, SeatStatus = "Locked", LockedByUserId = userId },
                new Seat { SeatId = 2, SeatNumber = 2, SeatStatus = "Locked", LockedByUserId = userId }
            };

            _mockSeatRepository.Setup(r => r.GetSeatsByNumbersAsync(scheduleId, seatNumbers))
                .ReturnsAsync(seats);

            // Act
            var result = await _seatService.UnlockSeatsAsync(scheduleId, seatNumbers, userId, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        #endregion
    }
}
