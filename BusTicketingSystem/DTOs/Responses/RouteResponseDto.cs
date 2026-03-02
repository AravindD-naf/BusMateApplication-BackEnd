namespace BusTicketingSystem.DTOs.Responses
{
    public class RouteResponseDto
    {
        public int RouteId { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        
  
        public decimal Distance { get; set; }


        public int EstimatedTravelTimeMinutes { get; set; }


        public decimal BaseFare { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
