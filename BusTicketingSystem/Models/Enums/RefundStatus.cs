namespace BusTicketingSystem.Models.Enums
{
    /// <summary>
    /// Refund processing status
    /// </summary>
    public enum RefundStatus
    {
        /// <summary>
        /// Refund requested, awaiting processing
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Refund is being processed
        /// </summary>
        Processing = 1,

        /// <summary>
        /// Refund successfully processed
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Refund failed
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Refund cancelled/rejected
        /// </summary>
        Rejected = 4
    }
}
