using BusTicketingSystem.Data;
using BusTicketingSystem.Interfaces.Repositories;
using BusTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingSystem.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Add Booking
        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        // ✅ Update Booking
        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await Task.CompletedTask;
        }

        // ✅ Get Booking By Id
        public async Task<Booking?> GetByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Schedule)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b =>
                    b.BookingId == bookingId &&
                    !b.IsDeleted);
        }

        // ✅ Get All Bookings (Admin)
        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Schedule)
                .Include(b => b.User)
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        // ✅ Get Bookings By User Id
        public async Task<List<Booking>> GetByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Schedule)
                .Where(b =>
                    b.UserId == userId &&
                    !b.IsDeleted)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        // ✅ Save Changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}