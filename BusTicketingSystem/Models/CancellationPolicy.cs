using System.ComponentModel.DataAnnotations;

namespace BusTicketingSystem.Models
{
    /// <summary>
    /// Cancellation policy rules for refunds
    /// </summary>
    public class CancellationPolicy
    {
        [Key]
        public int PolicyId { get; set; }

        /// <summary>
        /// Policy name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string PolicyName { get; set; } = "Standard";

        /// <summary>
        /// Hours before departure - minimum
        /// </summary>
        [Required]
        public int HoursBeforeDeparture { get; set; }

        /// <summary>
        /// Refund percentage for this window
        /// </summary>
        [Required]
        public int RefundPercentage { get; set; }

        /// <summary>
        /// Cancellation fee percentage
        /// </summary>
        [Required]
        public int CancellationFeePercentage { get; set; }

        /// <summary>
        /// Description of the policy
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Is this policy active?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// When policy was created
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When policy was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
