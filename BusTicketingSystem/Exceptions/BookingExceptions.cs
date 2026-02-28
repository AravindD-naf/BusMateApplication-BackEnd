namespace BusTicketingSystem.Exceptions
{
    /// <summary>
    /// Base exception for booking system
    /// </summary>
    public class BookingException : Exception
    {
        public BookingException(string message) : base(message) { }
        public BookingException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when schedule is invalid or not found
    /// </summary>
    public class InvalidScheduleException : BookingException
    {
        public InvalidScheduleException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when seats are not available
    /// </summary>
    public class InsufficientSeatsException : BookingException
    {
        public InsufficientSeatsException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when seat is not found
    /// </summary>
    public class SeatNotFoundException : BookingException
    {
        public SeatNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when seat status is invalid for operation
    /// </summary>
    public class InvalidSeatStatusException : BookingException
    {
        public InvalidSeatStatusException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when user is not authorized for operation
    /// </summary>
    public class UnauthorizedAccessException : BookingException
    {
        public UnauthorizedAccessException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when booking is not found
    /// </summary>
    public class BookingNotFoundException : BookingException
    {
        public BookingNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when booking cannot be cancelled
    /// </summary>
    public class BookingCancellationException : BookingException
    {
        public BookingCancellationException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when payment fails
    /// </summary>
    public class PaymentException : BookingException
    {
        public PaymentException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when payment timeout
    /// </summary>
    public class PaymentTimeoutException : PaymentException
    {
        public PaymentTimeoutException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when refund fails
    /// </summary>
    public class RefundException : BookingException
    {
        public RefundException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when passenger details are invalid
    /// </summary>
    public class InvalidPassengerException : BookingException
    {
        public InvalidPassengerException(string message) : base(message) { }
    }
}
