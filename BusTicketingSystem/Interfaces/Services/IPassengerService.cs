using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.DTOs.Responses;

namespace BusTicketingSystem.Interfaces.Services
{
    /// <summary>
    /// Service for managing passenger details in bookings
    /// </summary>
    public interface IPassengerService
    {
        /// <summary>
        /// Add passenger details to booking
        /// One passenger per seat
        /// </summary>
        Task<ApiResponse<List<PassengerResponseDto>>> AddPassengersAsync(
            AddPassengerRequestDto dto,
            int userId,
            string ipAddress);

        /// <summary>
        /// Get all passengers for a booking
        /// </summary>
        Task<ApiResponse<List<PassengerResponseDto>>> GetBookingPassengersAsync(int bookingId);

        /// <summary>
        /// Update passenger details
        /// </summary>
        Task<ApiResponse<PassengerResponseDto>> UpdatePassengerAsync(
            int passengerId,
            PassengerDetailDto dto,
            int userId,
            string ipAddress);

        /// <summary>
        /// Validate passenger details
        /// </summary>
        Task<ApiResponse<bool>> ValidatePassengersAsync(int bookingId);
    }
}
