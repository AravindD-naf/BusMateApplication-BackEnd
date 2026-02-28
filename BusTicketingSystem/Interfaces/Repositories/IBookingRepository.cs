using BusTicketingSystem.Models;

namespace BusTicketingSystem.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        // ✅ Create
        Task AddAsync(Booking booking);

        // ✅ Update
        Task UpdateAsync(Booking booking);

        // ✅ Get By Id
        Task<Booking?> GetByIdAsync(int bookingId);

        // ✅ Get All (Admin)
        Task<List<Booking>> GetAllAsync();

        // ✅ Get By User Id (User)
        Task<List<Booking>> GetByUserIdAsync(int userId);

        // ✅ Save
        Task SaveChangesAsync();
    }
}