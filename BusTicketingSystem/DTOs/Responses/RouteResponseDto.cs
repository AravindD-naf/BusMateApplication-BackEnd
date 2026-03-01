namespace BusTicketingSystem.DTOs.Responses
{
    public class RouteResponseDto
    {
        public int RouteId { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        
        /// <summary>
        /// Distance between source and destination in kilometers
        /// </summary>
        public decimal Distance { get; set; }

        /// <summary>
        /// Estimated travel time in minutes
        /// </summary>
        public int EstimatedTravelTimeMinutes { get; set; }

        /// <summary>
        /// Base fare for this route (per seat)
        /// </summary>
        public decimal BaseFare { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
