using BusTicketingSystem.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTicketingSystem.Models
{
    /// <summary>
    /// Refund record for cancelled bookings
    /// </summary>
    public class Refund
    {
        [Key]
        public int RefundId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!;

        [Required]
        public int PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; } = null!;

        /// <summary>
        /// Amount to be refunded (after deducting cancellation fee)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// Cancellation fee deducted
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CancellationFee { get; set; }

        /// <summary>
        /// Refund percentage (100%, 75%, 50%, 0% based on timing)
        /// </summary>
        [Required]
        public int RefundPercentage { get; set; }

        /// <summary>
        /// Current refund status
        /// </summary>
        [Required]
        public RefundStatus Status { get; set; } = RefundStatus.Pending;

        /// <summary>
        /// Reason for refund
        /// </summary>
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// When refund was requested
        /// </summary>
        [Required]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When refund was processed
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Soft delete flag
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
