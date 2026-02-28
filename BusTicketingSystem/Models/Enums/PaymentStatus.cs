namespace BusTicketingSystem.Models.Enums
{
    /// <summary>
    /// Payment processing status
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Payment initiated, awaiting processing
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Payment is being processed
        /// </summary>
        Processing = 1,

        /// <summary>
        /// Payment successful
        /// </summary>
        Success = 2,

        /// <summary>
        /// Payment failed (declined, timeout, etc.)
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Payment cancelled by user
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Refund initiated for this payment
        /// </summary>
        Refunded = 5
    }
}
