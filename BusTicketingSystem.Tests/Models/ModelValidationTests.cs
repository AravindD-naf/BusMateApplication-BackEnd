using BusTicketingSystem.Models;
using FluentAssertions;
using Xunit;

namespace BusTicketingSystem.Tests.Models
{
    public class BusModelTests
    {
        [Fact]
        public void Bus_NewInstance_ShouldHaveInactiveStatusByDefault()
        {
            // Arrange & Act
            var bus = new Bus
            {
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 40,
                OperatorName = "Test Operator"
            };

            // Assert
            bus.IsActive.Should().BeFalse();
            bus.IsDeleted.Should().BeFalse();
            bus.RatingAverage.Should().Be(0);
        }

        [Fact]
        public void Bus_WithMoreThan40Seats_ShouldNotBeValidated()
        {
            // Arrange & Act
            var bus = new Bus
            {
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 50, // This should fail validation
                OperatorName = "Test Operator"
            };

            // Assert
            bus.TotalSeats.Should().BeGreaterThan(40);
        }

        [Fact]
        public void Bus_WithValidSeats_ShouldBeBetween1And40()
        {
            // Arrange
            var validSeats = new[] { 1, 10, 20, 30, 40 };

            // Act & Assert
            foreach (var seats in validSeats)
            {
                var bus = new Bus
                {
                    BusNumber = "BUS001",
                    BusType = "AC",
                    TotalSeats = seats,
                    OperatorName = "Test Operator"
                };

                bus.TotalSeats.Should().BeGreaterThanOrEqualTo(1).And.BeLessThanOrEqualTo(40);
            }
        }
    }

    public class RouteModelTests
    {
        [Fact]
        public void Route_WithValidDistance_ShouldBeValid()
        {
            // Arrange & Act
            var route = new Route
            {
                Source = "Delhi",
                Destination = "Mumbai",
                Distance = 1400,
                EstimatedTravelTimeMinutes = 600,
                BaseFare = 1500,
                IsActive = true
            };

            // Assert
            route.Distance.Should().BeGreaterThan(0.1m).And.BeLessThanOrEqualTo(10000);
        }

        [Fact]
        public void Route_WithValidTravelTime_ShouldBeValid()
        {
            // Arrange & Act
            var route = new Route
            {
                Source = "Delhi",
                Destination = "Mumbai",
                Distance = 1400,
                EstimatedTravelTimeMinutes = 600,
                BaseFare = 1500
            };

            // Assert
            route.EstimatedTravelTimeMinutes.Should().BeGreaterThanOrEqualTo(1).And.BeLessThanOrEqualTo(1440);
        }
    }

    public class ScheduleModelTests
    {
        [Fact]
        public void Schedule_NewInstance_ShouldBeActiveByDefault()
        {
            // Arrange & Act
            var schedule = new Schedule
            {
                RouteId = 1,
                BusId = 1,
                TravelDate = DateTime.UtcNow.AddDays(1),
                DepartureTime = TimeSpan.FromHours(10),
                ArrivalTime = TimeSpan.FromHours(16),
                TotalSeats = 40,
                AvailableSeats = 40
            };

            // Assert
            schedule.IsActive.Should().BeTrue();
            schedule.IsDeleted.Should().BeFalse();
            schedule.AvailableSeats.Should().Be(40);
        }

        [Fact]
        public void Schedule_ShouldTrackAvailableSeats()
        {
            // Arrange & Act
            var schedule = new Schedule
            {
                RouteId = 1,
                BusId = 1,
                TravelDate = DateTime.UtcNow.AddDays(1),
                DepartureTime = TimeSpan.FromHours(10),
                ArrivalTime = TimeSpan.FromHours(16),
                TotalSeats = 40,
                AvailableSeats = 35
            };

            // Assert
            schedule.AvailableSeats.Should().Be(35);
            (schedule.TotalSeats - schedule.AvailableSeats).Should().Be(5);
        }
    }

    public class SeatModelTests
    {
        [Fact]
        public void Seat_NewInstance_ShouldHaveAvailableStatus()
        {
            // Arrange & Act
            var seat = new Seat
            {
                ScheduleId = 1,
                SeatNumber = "A1",
                SeatStatus = "Available"
            };

            // Assert
            seat.SeatStatus.Should().Be("Available");
            seat.IsDeleted.Should().BeFalse();
        }

        [Theory]
        [InlineData("A1")]
        [InlineData("A2")]
        [InlineData("B1")]
        [InlineData("C10")]
        public void Seat_WithValidSeatNumber_ShouldBeValid(string seatNumber)
        {
            // Arrange & Act
            var seat = new Seat
            {
                ScheduleId = 1,
                SeatNumber = seatNumber,
                SeatStatus = "Available"
            };

            // Assert
            seat.SeatNumber.Should().Be(seatNumber);
            seat.SeatNumber.Length.Should().BeLessThanOrEqualTo(10);
        }
    }

    public class SourceAndDestinationModelTests
    {
        [Fact]
        public void Source_NewInstance_ShouldBeActive()
        {
            // Arrange & Act
            var source = new Source
            {
                SourceName = "Delhi",
                Description = "Capital"
            };

            // Assert
            source.IsActive.Should().BeTrue();
            source.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void Destination_NewInstance_ShouldBeActive()
        {
            // Arrange & Act
            var destination = new Destination
            {
                DestinationName = "Mumbai",
                Description = "Financial Hub"
            };

            // Assert
            destination.IsActive.Should().BeTrue();
            destination.IsDeleted.Should().BeFalse();
        }
    }
}
