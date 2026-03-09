using BusTicketingSystem.DTOs.Requests;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace BusTicketingSystem.Tests.DTOs
{
    public class CreateBusRequestValidationTests
    {
        [Fact]
        public void CreateBusRequest_WithValidData_ShouldPass()
        {
            // Arrange
            var request = new CreateBusRequest
            {
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 40,
                OperatorName = "Test Operator"
            };

            // Act
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void CreateBusRequest_WithZeroSeats_ShouldFail()
        {
            // Arrange
            var request = new CreateBusRequest
            {
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 0,
                OperatorName = "Test Operator"
            };

            // Act
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateBusRequest_WithMoreThan40Seats_ShouldFail()
        {
            // Arrange
            var request = new CreateBusRequest
            {
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 50,
                OperatorName = "Test Operator"
            };

            // Act
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().NotBeEmpty();
        }
    }

    public class PaginationRequestValidationTests
    {
        [Fact]
        public void PaginationRequest_WithDefaultValues_ShouldHaveValidDefaults()
        {
            // Arrange & Act
            var request = new PaginationRequest();

            // Assert
            request.PageNumber.Should().Be(1);
            request.PageSize.Should().Be(10);
        }

        [Fact]
        public void PaginationRequest_WithCustomValues_ShouldBeValid()
        {
            // Arrange
            var request = new PaginationRequest
            {
                PageNumber = 2,
                PageSize = 20
            };

            // Act & Assert
            request.PageNumber.Should().Be(2);
            request.PageSize.Should().Be(20);
        }
    }

    public class RouteCreateRequestValidationTests
    {
        [Fact]
        public void RouteCreateRequest_WithValidData_ShouldPass()
        {
            // Arrange
            var request = new RouteCreateRequestDto
            {
                Source = "Delhi",
                Destination = "Mumbai",
                Distance = 1400,
                EstimatedTravelTimeMinutes = 600,
                BaseFare = 1500
            };

            // Act
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void RouteCreateRequest_WithZeroDistance_ShouldFail()
        {
            // Arrange
            var request = new RouteCreateRequestDto
            {
                Source = "Delhi",
                Destination = "Mumbai",
                Distance = 0,
                EstimatedTravelTimeMinutes = 600,
                BaseFare = 1500
            };

            // Act
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            isValid.Should().BeFalse();
        }
    }

    public class ScheduleRequestValidationTests
    {
        [Fact]
        public void ScheduleRequest_WithValidData_ShouldPass()
        {
            // Arrange
            var request = new ScheduleRequestDto
            {
                RouteId = 1,
                BusId = 1,
                TravelDate = DateTime.UtcNow.AddDays(1),
                DepartureTime = TimeSpan.FromHours(10),
                ArrivalTime = TimeSpan.FromHours(16)
            };

            // Act
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            isValid.Should().BeTrue();
        }
    }
}
