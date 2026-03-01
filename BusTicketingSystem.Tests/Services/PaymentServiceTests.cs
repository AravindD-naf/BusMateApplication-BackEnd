using BusTicketingSystem.DTOs.Requests;
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
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentRepository> _mockPaymentRepository;
        private readonly Mock<IRefundRepository> _mockRefundRepository;
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<IScheduleRepository> _mockScheduleRepository;
        private readonly Mock<ICancellationPolicyRepository> _mockPolicyRepository;
        private readonly Mock<IAuditRepository> _mockAuditRepository;
        private readonly PaymentService _paymentService;

        public PaymentServiceTests()
        {
            _mockPaymentRepository = new Mock<IPaymentRepository>();
            _mockRefundRepository = new Mock<IRefundRepository>();
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockScheduleRepository = new Mock<IScheduleRepository>();
            _mockPolicyRepository = new Mock<ICancellationPolicyRepository>();
            _mockAuditRepository = new Mock<IAuditRepository>();

            _paymentService = new PaymentService(
                _mockPaymentRepository.Object,
                _mockRefundRepository.Object,
                _mockBookingRepository.Object,
                _mockScheduleRepository.Object,
                _mockPolicyRepository.Object,
                _mockAuditRepository.Object
            );
        }

        #region InitiatePaymentAsync Tests

        [Fact]
        public async Task InitiatePaymentAsync_WithValidInput_ShouldCreatePayment()
        {
            // Arrange
            var bookingId = 1;
            var amount = 500m;
            var userId = 1;
            var ipAddress = "127.0.0.1";

            var booking = new Booking
            {
                BookingId = bookingId,
                UserId = userId,
                TotalAmount = amount,
                BookingStatus = BookingStatus.Pending
            };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId))
                .ReturnsAsync(booking);

            // Act
            var result = await _paymentService.InitiatePaymentAsync(bookingId, amount, userId, ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockPaymentRepository.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Once);
            _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task InitiatePaymentAsync_WithInvalidBooking_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Booking)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _paymentService.InitiatePaymentAsync(999, 500m, 1, "127.0.0.1"));
        }

        [Fact]
        public async Task InitiatePaymentAsync_WithWrongUser_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var booking = new Booking
            {
                BookingId = 1,
                UserId = 1,
                TotalAmount = 500m,
                BookingStatus = BookingStatus.Pending
            };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(booking);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _paymentService.InitiatePaymentAsync(1, 500m, 2, "127.0.0.1"));
        }

        [Fact]
        public async Task InitiatePaymentAsync_WithWrongAmount_ShouldThrowPaymentException()
        {
            // Arrange
            var booking = new Booking
            {
                BookingId = 1,
                UserId = 1,
                TotalAmount = 500m,
                BookingStatus = BookingStatus.Pending
            };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(booking);

            // Act & Assert
            await Assert.ThrowsAsync<PaymentOperationException>(() =>
                _paymentService.InitiatePaymentAsync(1, 600m, 1, "127.0.0.1"));
        }

        #endregion

        #region GetPaymentAsync Tests

        [Fact]
        public async Task GetPaymentAsync_WithValidId_ShouldReturnPayment()
        {
            // Arrange
            var paymentId = 1;
            var payment = new Payment
            {
                PaymentId = paymentId,
                Amount = 500m,
                Status = PaymentStatus.Pending
            };

            _mockPaymentRepository.Setup(r => r.GetByIdAsync(paymentId))
                .ReturnsAsync(payment);

            // Act
            var result = await _paymentService.GetPaymentAsync(paymentId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Amount.Should().Be(500m);
        }

        [Fact]
        public async Task GetPaymentAsync_WithInvalidId_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            _mockPaymentRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Payment)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _paymentService.GetPaymentAsync(999));
        }

        #endregion

        #region ConfirmPaymentAsync Tests

        [Fact]
        public async Task ConfirmPaymentAsync_WithValidInput_ShouldConfirmPayment()
        {
            // Arrange
            var dto = new ConfirmPaymentRequestDto
            {
                PaymentId = 1,
                TransactionId = "TXN123",
                IsSuccess = true
            };

            var payment = new Payment
            {
                PaymentId = 1,
                BookingId = 1,
                Status = PaymentStatus.Pending
            };

            var booking = new Booking
            {
                BookingId = 1,
                UserId = 1,
                BookingStatus = BookingStatus.PaymentProcessing
            };

            _mockPaymentRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(payment);

            _mockBookingRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(booking);

            // Act
            var result = await _paymentService.ConfirmPaymentAsync(dto, 1, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockPaymentRepository.Verify(r => r.UpdateAsync(It.IsAny<Payment>()), Times.Once);
        }

        [Fact]
        public async Task ConfirmPaymentAsync_WithInvalidPayment_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            var dto = new ConfirmPaymentRequestDto
            {
                PaymentId = 999,
                TransactionId = "TXN123",
                IsSuccess = true
            };

            _mockPaymentRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Payment)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _paymentService.ConfirmPaymentAsync(dto, 1, "127.0.0.1"));
        }

        #endregion

        #region InitiateRefundAsync Tests

        [Fact]
        public async Task InitiateRefundAsync_WithValidInput_ShouldCreateRefund()
        {
            // Arrange
            var bookingId = 1;
            var userId = 1;

            var booking = new Booking
            {
                BookingId = bookingId,
                UserId = userId,
                ScheduleId = 1,
                TotalAmount = 500m,
                CancellationReason = "Personal reasons"
            };

            var payment = new Payment
            {
                PaymentId = 1,
                BookingId = bookingId,
                Status = PaymentStatus.Success,
                Amount = 500m
            };

            var schedule = new Schedule
            {
                ScheduleId = 1,
                TravelDate = DateTime.UtcNow.AddDays(2),
                DepartureTime = TimeSpan.FromHours(10)
            };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId))
                .ReturnsAsync(booking);

            _mockPaymentRepository.Setup(r => r.GetByBookingIdAsync(bookingId))
                .ReturnsAsync(payment);

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(schedule);

            _mockPolicyRepository.Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(new List<CancellationPolicy>());

            // Act
            var result = await _paymentService.InitiateRefundAsync(bookingId, userId, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockRefundRepository.Verify(r => r.AddAsync(It.IsAny<Refund>()), Times.Once);
        }

        [Fact]
        public async Task InitiateRefundAsync_WithInvalidBooking_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Booking)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _paymentService.InitiateRefundAsync(999, 1, "127.0.0.1"));
        }

        #endregion

        #region CalculateRefundAsync Tests

        [Fact]
        public async Task CalculateRefundAsync_With48HoursOrMore_ShouldReturn100Percent()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking
            {
                BookingId = bookingId,
                ScheduleId = 1,
                TotalAmount = 1000m
            };

            var schedule = new Schedule
            {
                ScheduleId = 1,
                TravelDate = DateTime.UtcNow.AddDays(3),
                DepartureTime = TimeSpan.FromHours(10)
            };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId))
                .ReturnsAsync(booking);

            _mockScheduleRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(schedule);

            _mockPolicyRepository.Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(new List<CancellationPolicy>());

            // Act
            var result = await _paymentService.CalculateRefundAsync(bookingId);

            // Assert
            result.refundPercentage.Should().Be(100);
            result.refundAmount.Should().Be(1000m);
        }

        #endregion
    }
}
