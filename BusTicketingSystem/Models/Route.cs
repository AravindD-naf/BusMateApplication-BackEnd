using System.ComponentModel.DataAnnotations;

namespace BusTicketingSystem.Models
{
    public class Route
    {
        public int RouteId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Source { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Destination { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}