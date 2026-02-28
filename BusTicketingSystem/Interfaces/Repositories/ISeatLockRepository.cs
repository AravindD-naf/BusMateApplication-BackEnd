using BusTicketingSystem.Models;

namespace BusTicketingSystem.Interfaces.Repositories
{
    public interface ISeatLockRepository
    {
        // Create lock
        Task AddAsync(SeatLock seatLock);

        // Get active lock for seat
        Task<SeatLock?> GetActiveLockAsync(int seatId);

        // Get locks by user
        Task<List<SeatLock>> GetUserLocksAsync(int scheduleId, int userId);

        // Release lock
        Task UpdateAsync(SeatLock seatLock);

        // Get by id
        Task<SeatLock?> GetByIdAsync(int seatLockId);

        // Check if user has lock on specific seat
        Task<bool> HasActiveLockAsync(int seatId, int userId);

        // Clean up expired locks
        Task<int> CleanupExpiredLocksAsync();

        // Get expired locks
        Task<List<SeatLock>> GetExpiredLocksAsync();

        // Save changes
        Task SaveChangesAsync();
    }
}
