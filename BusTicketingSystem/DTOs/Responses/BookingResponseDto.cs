namespace BusTicketingSystem.DTOs.Responses
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public int ScheduleId { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalAmount { get; set; }
        public string BookingStatus { get; set; }
        public DateTime BookingDate { get; set; }
    }

    public class BookingDetailResponseDto
    {
        public int BookingId { get; set; }
        public int ScheduleId { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalAmount { get; set; }
        public string BookingStatus { get; set; }
        public DateTime BookingDate { get; set; }

        // Route Details
        public int RouteId { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }

        // Bus Details
        public int BusId { get; set; }
        public string BusNumber { get; set; }
        public string BusType { get; set; }
        public int TotalSeats { get; set; }
        public string OperatorName { get; set; }
        public double RatingAverage { get; set; }

        // Schedule Details
        public DateTime TravelDate { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public int AvailableSeats { get; set; }
    }
}
