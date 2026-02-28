using BusTicketingSystem.DTOs.Responses;

namespace BusTicketingSystem.Interfaces.Services
{
    public interface ISeatService
    {
        /// <summary>
        /// Get complete seat layout for a schedule
        /// </summary>
        Task<ApiResponse<SeatLayoutResponseDto>> GetSeatLayoutAsync(int scheduleId);

        /// <summary>
        /// Lock seats for a user with 5-minute expiry
        /// </summary>
        Task<ApiResponse<LockSeatsResponseDto>> LockSeatsAsync(
            int scheduleId,
            List<string> seatNumbers,
            int userId,
            string ipAddress);

        /// <summary>
        /// Release locked seats by user
        /// </summary>
        Task<ApiResponse<ReleaseSeatsResponseDto>> ReleaseSeatsAsync(
            int scheduleId,
            List<string> seatNumbers,
            int userId,
            string ipAddress);

        /// <summary>
        /// Cleanup expired locks (can be called by background job)
        /// </summary>
        Task<int> CleanupExpiredLocksAsync();

        /// <summary>
        /// Confirm booking - converts locked seats to booked
        /// </summary>
        Task<ApiResponse<bool>> ConfirmBookingSeatsAsync(
            int bookingId,
            int scheduleId,
            List<string> seatNumbers,
            int userId);

        /// <summary>
        /// Release booked seats when cancelling booking
        /// </summary>
        Task<ApiResponse<bool>> ReleaseBookingSeatsAsync(
            int scheduleId,
            List<string> seatNumbers);
    }
}
