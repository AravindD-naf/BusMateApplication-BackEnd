using BusTicketingSystem.Data;
using BusTicketingSystem.Models;
using BusTicketingSystem.Models.Enums;
using BusTicketingSystem.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BusTicketingSystem.Tests.Repositories
{
    public class BookingRepositoryTests : IAsyncLifetime
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private ApplicationDbContext _context;
        private BookingRepository _repository;

        public BookingRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BusTicketingSystemTest_" + Guid.NewGuid())
                .Options;
        }

        public async Task InitializeAsync()
        {
            _context = new ApplicationDbContext(_dbContextOptions);
            await _context.Database.EnsureCreatedAsync();
            _repository = new BookingRepository(_context);
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_WithValidBooking_ShouldAddBooking()
        {
            // Arrange
            var booking = new Booking
            {
                UserId = 1,
                ScheduleId = 1,
                NumberOfSeats = 2,
                TotalAmount = 1000m,
                BookingStatus = BookingStatus.Pending,
                BookingDate = DateTime.UtcNow
            };

            // Act
            await _repository.AddAsync(booking);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _context.Bookings.FindAsync(booking.BookingId);
            result.Should().NotBeNull();
            result.NumberOfSeats.Should().Be(2);
            result.TotalAmount.Should().Be(1000m);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnBooking()
        {
            // Arrange
            var booking = new Booking
            {
                UserId = 1,
                ScheduleId = 1,
                NumberOfSeats = 2,
                TotalAmount = 1000m,
                BookingStatus = BookingStatus.Pending,
                BookingDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(booking.BookingId);

            // Assert
            result.Should().NotBeNull();
            result.BookingId.Should().Be(booking.BookingId);
            result.NumberOfSeats.Should().Be(2);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetByUserIdAsync Tests

        [Fact]
        public async Task GetByUserIdAsync_WithValidUserId_ShouldReturnUserBookings()
        {
            // Arrange
            var userId = 1;
            var bookings = new List<Booking>
            {
                new Booking { UserId = userId, ScheduleId = 1, NumberOfSeats = 2, TotalAmount = 1000m, BookingStatus = BookingStatus.Pending, BookingDate = DateTime.UtcNow },
                new Booking { UserId = userId, ScheduleId = 2, NumberOfSeats = 1, TotalAmount = 500m, BookingStatus = BookingStatus.Confirmed, BookingDate = DateTime.UtcNow }
            };

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUserIdAsync(userId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(b => b.UserId.Should().Be(userId));
        }

        [Fact]
        public async Task GetByUserIdAsync_WithNonExistingUserId_ShouldReturnEmptyList()
        {
            // Act
            var result = await _repository.GetByUserIdAsync(999);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidBooking_ShouldUpdateBooking()
        {
            // Arrange
            var booking = new Booking
            {
                UserId = 1,
                ScheduleId = 1,
                NumberOfSeats = 2,
                TotalAmount = 1000m,
                BookingStatus = BookingStatus.Pending,
                BookingDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            booking.BookingStatus = BookingStatus.Confirmed;
            await _repository.UpdateAsync(booking);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _context.Bookings.FindAsync(booking.BookingId);
            result.BookingStatus.Should().Be(BookingStatus.Confirmed);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBookings()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking { UserId = 1, ScheduleId = 1, NumberOfSeats = 2, TotalAmount = 1000m, BookingStatus = BookingStatus.Pending, BookingDate = DateTime.UtcNow },
                new Booking { UserId = 2, ScheduleId = 2, NumberOfSeats = 1, TotalAmount = 500m, BookingStatus = BookingStatus.Confirmed, BookingDate = DateTime.UtcNow }
            };

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteBooking()
        {
            // Arrange
            var booking = new Booking
            {
                UserId = 1,
                ScheduleId = 1,
                NumberOfSeats = 2,
                TotalAmount = 1000m,
                BookingStatus = BookingStatus.Pending,
                BookingDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(booking.BookingId);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _context.Bookings.FindAsync(booking.BookingId);
            result.Should().BeNull();
        }

        #endregion
    }
}
