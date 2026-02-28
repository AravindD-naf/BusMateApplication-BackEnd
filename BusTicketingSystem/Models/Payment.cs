using BusTicketingSystem.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTicketingSystem.Models
{
    /// <summary>
    /// Payment record for booking
    /// </summary>
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!;

        /// <summary>
        /// Payment amount (should match booking total)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Current payment status
        /// </summary>
        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        /// <summary>
        /// Unique transaction ID (from payment gateway or mock)
        /// </summary>
        [MaxLength(100)]
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Payment method used (Card, UPI, Wallet, Bank Transfer, etc.)
        /// </summary>
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>
        /// Reason for payment failure (if failed)
        /// </summary>
        [MaxLength(500)]
        public string FailureReason { get; set; } = string.Empty;

        /// <summary>
        /// When payment was initiated
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When payment was processed
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// When payment expires if still pending (default 15 min)
        /// </summary>
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(15);

        /// <summary>
        /// Soft delete flag
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public virtual Refund? Refund { get; set; }
    }
}
