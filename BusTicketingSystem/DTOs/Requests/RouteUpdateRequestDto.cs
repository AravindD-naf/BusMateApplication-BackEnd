using System.ComponentModel.DataAnnotations;

namespace BusTicketingSystem.DTOs.Requests
{
    public class RouteUpdateRequestDto
    {
        [Required]
        [MaxLength(150)]
        public string Source { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Destination { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}