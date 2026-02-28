using BusTicketingSystem.Models;

namespace BusTicketingSystem.Interfaces.Repositories
{
    public interface ISeatRepository
    {
        // Get seats by schedule
        Task<List<Seat>> GetSeatsByScheduleIdAsync(int scheduleId);

        // Get single seat
        Task<Seat?> GetByIdAsync(int seatId);

        // Get seats by numbers
        Task<List<Seat>> GetSeatsByNumbersAsync(int scheduleId, List<string> seatNumbers);

        // Get seat by schedule and number
        Task<Seat?> GetSeatByScheduleAndNumberAsync(int scheduleId, string seatNumber);

        // Add/Update
        Task AddAsync(Seat seat);
        Task UpdateAsync(Seat seat);
        Task UpdateManyAsync(List<Seat> seats);

        // Check seat availability
        Task<bool> IsAvailableAsync(int seatId);

        // Get locked seats by user
        Task<List<Seat>> GetLockedSeatsByUserAsync(int scheduleId, int userId);

        // Clean up expired locks
        Task<int> CleanupExpiredLocksAsync();

        // Save changes
        Task SaveChangesAsync();
    }
}
