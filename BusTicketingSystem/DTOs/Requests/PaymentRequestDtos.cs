namespace BusTicketingSystem.DTOs.Requests
{
    /// <summary>
    /// Request DTO for processing payment
    /// </summary>
    public class ProcessPaymentRequestDto
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // Card, UPI, Wallet, etc.
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// Request DTO for confirming payment (after user completes payment)
    /// </summary>
    public class ConfirmPaymentRequestDto
    {
        public int PaymentId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string FailureReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for cancelling a booking and requesting refund
    /// </summary>
    public class CancelBookingWithRefundRequestDto
    {
        public int BookingId { get; set; }
        public string CancellationReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for confirming refund
    /// </summary>
    public class ConfirmRefundRequestDto
    {
        public int RefundId { get; set; }
        public bool IsApproved { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO to add passenger details to booking
    /// </summary>
    public class AddPassengerRequestDto
    {
        public int BookingId { get; set; }
        public List<PassengerDetailDto> Passengers { get; set; } = new();
    }

    /// <summary>
    /// Passenger details
    /// </summary>
    public class PassengerDetailDto
    {
        public string SeatNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IdType { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public int Age { get; set; }
        public string SpecialRequirements { get; set; } = string.Empty;
    }
}
