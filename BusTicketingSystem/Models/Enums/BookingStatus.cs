namespace BusTicketingSystem.Models.Enums
{
    /// <summary>
    /// Booking lifecycle status
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// Seats locked, awaiting payment
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Payment is being processed
        /// </summary>
        PaymentProcessing = 1,

        /// <summary>
        /// Payment successful, booking confirmed
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// Payment failed or was rejected
        /// </summary>
        PaymentFailed = 3,

        /// <summary>
        /// User cancelled the booking
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Booking expired (payment timeout)
        /// </summary>
        Expired = 5,

        /// <summary>
        /// Refund processed for cancelled booking
        /// </summary>
        Refunded = 6
    }
}
