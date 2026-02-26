using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTicketingSystem.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }

        // 🔗 Foreign Keys
        public int BusId { get; set; }
        public Bus Bus { get; set; } = null!;

        public int RouteId { get; set; }
        public Route Route { get; set; } = null!;

        // 🕒 Timing
        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        // 💰 Pricing
        [Column(TypeName = "decimal(10,2)")]
        public decimal Fare { get; set; }

        // 🪑 Seat Tracking (initially equals Bus.TotalSeats)
        public int AvailableSeats { get; set; }

        // 🔒 Status
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // 🧾 Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}