using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTicketingSystem.Models
{
    /// <summary>
    /// Passenger details for each seat in a booking
    /// </summary>
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!;

        [Required]
        public int SeatId { get; set; }

        [ForeignKey("SeatId")]
        public Seat Seat { get; set; } = null!;

        /// <summary>
        /// Seat number assigned to this passenger
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string SeatNumber { get; set; } = string.Empty;

        /// <summary>
        /// Passenger's first name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Passenger's last name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Passenger's phone number
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Passenger's email
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// ID type (Passport, Aadhaar, DrivingLicense, etc.)
        /// </summary>
        [MaxLength(50)]
        public string IdType { get; set; } = string.Empty;

        /// <summary>
        /// ID number
        /// </summary>
        [MaxLength(50)]
        public string IdNumber { get; set; } = string.Empty;

        /// <summary>
        /// Passenger age
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Special requirements (wheelchair, etc.)
        /// </summary>
        [MaxLength(500)]
        public string SpecialRequirements { get; set; } = string.Empty;

        /// <summary>
        /// When passenger was added
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When passenger details were updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Soft delete flag
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
