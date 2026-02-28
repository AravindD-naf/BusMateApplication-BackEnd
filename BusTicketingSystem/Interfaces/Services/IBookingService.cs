using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.DTOs.Responses;

namespace BusTicketingSystem.Interfaces.Services
{
    public interface IBookingService
    {
        /// <summary>
        /// Create a new booking with locked seats
        /// Requires seats to be locked by user beforehand
        /// </summary>
        Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(
            CreateBookingRequestDto dto,
            int userId,
            string ipAddress);

        Task<ApiResponse<List<BookingResponseDto>>>
            GetMyBookingsAsync(int userId);

        Task<ApiResponse<List<BookingResponseDto>>>
            GetAllBookingsAsync();

        Task<ApiResponse<BookingDetailResponseDto>> GetBookingByIdAsync(int bookingId);

        /// <summary>
        /// Cancel booking and release booked seats back to available
        /// </summary>
        Task<ApiResponse<bool>> CancelBookingAsync(
            int bookingId,
            int userId,
            string role,
            string ipAddress);
    }
}
