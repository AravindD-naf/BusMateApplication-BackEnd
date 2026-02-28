using BusTicketingSystem.DTOs.Responses;
using BusTicketingSystem.Interfaces.Repositories;
using BusTicketingSystem.Interfaces.Services;
using BusTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingSystem.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly ISeatLockRepository _seatLockRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IAuditRepository _auditRepository;
        private const int LOCK_EXPIRY_MINUTES = 5;

        public SeatService(
            ISeatRepository seatRepository,
            ISeatLockRepository seatLockRepository,
            IScheduleRepository scheduleRepository,
            IAuditRepository auditRepository)
        {
            _seatRepository = seatRepository;
            _seatLockRepository = seatLockRepository;
            _scheduleRepository = scheduleRepository;
            _auditRepository = auditRepository;
        }

        public async Task<ApiResponse<SeatLayoutResponseDto>> GetSeatLayoutAsync(int scheduleId)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null || schedule.IsDeleted || !schedule.IsActive)
                throw new Exception("Invalid schedule.");

            var seats = await _seatRepository.GetSeatsByScheduleIdAsync(scheduleId);

            var seatLayout = new SeatLayoutResponseDto
            {
                ScheduleId = scheduleId,
                BusId = schedule.BusId,
                BusNumber = schedule.Bus?.BusNumber ?? string.Empty,
                TotalSeats = schedule.TotalSeats,
                AvailableSeats = schedule.AvailableSeats,
                LockedSeats = seats.Count(s => s.SeatStatus == "Locked"),
                BookedSeats = seats.Count(s => s.SeatStatus == "Booked"),
                Seats = seats.Select(s => new SeatResponseDto
                {
                    SeatId = s.SeatId,
                    SeatNumber = s.SeatNumber,
                    SeatStatus = s.SeatStatus,
                    LockedByUserId = s.LockedByUserId,
                    LockedAt = s.LockedAt,
                    LockedExpiresAt = s.LockedAt?.AddMinutes(LOCK_EXPIRY_MINUTES)
                }).ToList()
            };

            return ApiResponse<SeatLayoutResponseDto>.SuccessResponse(seatLayout);
        }

        public async Task<ApiResponse<LockSeatsResponseDto>> LockSeatsAsync(
            int scheduleId,
            List<string> seatNumbers,
            int userId,
            string ipAddress)
        {
            if (seatNumbers == null || seatNumbers.Count == 0)
                throw new Exception("At least one seat must be selected.");

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null || schedule.IsDeleted || !schedule.IsActive)
                throw new Exception("Invalid schedule.");

            // Check travel date - cannot lock after departure
            DateTime departureDateTime = schedule.TravelDate.Add(schedule.DepartureTime);
            if (departureDateTime <= DateTime.UtcNow)
                throw new Exception("Cannot lock seats after departure time.");

            var response = new LockSeatsResponseDto
            {
                LockedSeatNumbers = new List<string>(),
                FailedSeatNumbers = new List<string>()
            };

            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(LOCK_EXPIRY_MINUTES);

            // Clean up expired locks first
            await _seatLockRepository.CleanupExpiredLocksAsync();

            using (var transaction = await _scheduleRepository.BeginTransactionAsync())
            {
                try
                {
                    var seats = await _seatRepository.GetSeatsByNumbersAsync(scheduleId, seatNumbers);

                    // Validate each seat
                    foreach (var seatNumber in seatNumbers)
                    {
                        var seat = seats.FirstOrDefault(s => s.SeatNumber == seatNumber);

                        if (seat == null)
                        {
                            response.FailedSeatNumbers.Add($"{seatNumber} (not found)");
                            continue;
                        }

                        if (seat.SeatStatus == "Booked")
                        {
                            response.FailedSeatNumbers.Add($"{seatNumber} (already booked)");
                            continue;
                        }

                        if (seat.SeatStatus == "Locked" && seat.LockedByUserId != userId)
                        {
                            response.FailedSeatNumbers.Add($"{seatNumber} (locked by another user)");
                            continue;
                        }

                        // If already locked by same user, update expiry
                        if (seat.SeatStatus == "Locked" && seat.LockedByUserId == userId)
                        {
                            seat.LockedAt = now;
                            seat.UpdatedAt = now;
                            response.LockedSeatNumbers.Add(seatNumber);
                            continue;
                        }

                        // Lock the seat
                        seat.SeatStatus = "Locked";
                        seat.LockedByUserId = userId;
                        seat.LockedAt = now;
                        seat.UpdatedAt = now;

                        // Create seat lock record
                        var seatLock = new SeatLock
                        {
                            SeatId = seat.SeatId,
                            UserId = userId,
                            LockedAt = now,
                            ExpiresAt = expiresAt
                        };

                        await _seatLockRepository.AddAsync(seatLock);
                        response.LockedSeatNumbers.Add(seatNumber);
                    }

                    // Update all seats
                    var seatsToUpdate = seats.Where(s => response.LockedSeatNumbers.Contains(s.SeatNumber)).ToList();
                    await _seatRepository.UpdateManyAsync(seatsToUpdate);
                    await _seatLockRepository.SaveChangesAsync();
                    await _seatRepository.SaveChangesAsync();

                    await transaction.CommitAsync();

                    response.Success = response.FailedSeatNumbers.Count == 0;
                    response.LockExpiresAt = expiresAt;
                    response.Message = response.Success
                        ? $"All {response.LockedSeatNumbers.Count} seats locked successfully."
                        : $"Locked {response.LockedSeatNumbers.Count} seats. Failed: {response.FailedSeatNumbers.Count}";

                    // Audit log
                    await _auditRepository.LogAuditAsync(
                        "LOCK_SEATS",
                        "Seat",
                        string.Join(",", response.LockedSeatNumbers),
                        null,
                        new { scheduleId, seatNumbers = response.LockedSeatNumbers },
                        userId,
                        ipAddress);

                    return ApiResponse<LockSeatsResponseDto>.SuccessResponse(response);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<ApiResponse<ReleaseSeatsResponseDto>> ReleaseSeatsAsync(
            int scheduleId,
            List<string> seatNumbers,
            int userId,
            string ipAddress)
        {
            if (seatNumbers == null || seatNumbers.Count == 0)
                throw new Exception("At least one seat must be specified.");

            var response = new ReleaseSeatsResponseDto
            {
                ReleasedSeatNumbers = new List<string>(),
                FailedSeatNumbers = new List<string>()
            };

            using (var transaction = await _scheduleRepository.BeginTransactionAsync())
            {
                try
                {
                    var seats = await _seatRepository.GetSeatsByNumbersAsync(scheduleId, seatNumbers);

                    foreach (var seatNumber in seatNumbers)
                    {
                        var seat = seats.FirstOrDefault(s => s.SeatNumber == seatNumber);

                        if (seat == null)
                        {
                            response.FailedSeatNumbers.Add($"{seatNumber} (not found)");
                            continue;
                        }

                        if (seat.SeatStatus != "Locked")
                        {
                            response.FailedSeatNumbers.Add($"{seatNumber} (not locked)");
                            continue;
                        }

                        if (seat.LockedByUserId != userId)
                        {
                            response.FailedSeatNumbers.Add($"{seatNumber} (locked by another user)");
                            continue;
                        }

                        // Release the seat
                        seat.SeatStatus = "Available";
                        seat.LockedByUserId = null;
                        seat.LockedAt = null;
                        seat.UpdatedAt = DateTime.UtcNow;

                        response.ReleasedSeatNumbers.Add(seatNumber);
                    }

                    // Update seats
                    var seatsToUpdate = seats.Where(s => response.ReleasedSeatNumbers.Contains(s.SeatNumber)).ToList();
                    await _seatRepository.UpdateManyAsync(seatsToUpdate);
                    await _seatRepository.SaveChangesAsync();

                    await transaction.CommitAsync();

                    response.Success = response.FailedSeatNumbers.Count == 0;
                    response.Message = response.Success
                        ? $"All {response.ReleasedSeatNumbers.Count} seats released successfully."
                        : $"Released {response.ReleasedSeatNumbers.Count} seats. Failed: {response.FailedSeatNumbers.Count}";

                    // Audit log
                    await _auditRepository.LogAuditAsync(
                        "RELEASE_SEATS",
                        "Seat",
                        string.Join(",", response.ReleasedSeatNumbers),
                        null,
                        new { scheduleId, seatNumbers = response.ReleasedSeatNumbers },
                        userId,
                        ipAddress);

                    return ApiResponse<ReleaseSeatsResponseDto>.SuccessResponse(response);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<int> CleanupExpiredLocksAsync()
        {
            return await _seatLockRepository.CleanupExpiredLocksAsync();
        }

        public async Task<ApiResponse<bool>> ConfirmBookingSeatsAsync(
            int bookingId,
            int scheduleId,
            List<string> seatNumbers,
            int userId)
        {
            if (seatNumbers == null || seatNumbers.Count == 0)
                throw new Exception("At least one seat must be confirmed.");

            using (var transaction = await _scheduleRepository.BeginTransactionAsync())
            {
                try
                {
                    var seats = await _seatRepository.GetSeatsByNumbersAsync(scheduleId, seatNumbers);

                    // Validate all seats are locked by this user
                    foreach (var seatNumber in seatNumbers)
                    {
                        var seat = seats.FirstOrDefault(s => s.SeatNumber == seatNumber);

                        if (seat == null)
                            throw new Exception($"Seat {seatNumber} not found.");

                        if (seat.SeatStatus != "Locked")
                            throw new Exception($"Seat {seatNumber} is not locked.");

                        if (seat.LockedByUserId != userId)
                            throw new Exception($"Seat {seatNumber} is not locked by you.");

                        // Convert to booked
                        seat.SeatStatus = "Booked";
                        seat.BookingId = bookingId;
                        seat.LockedByUserId = null;
                        seat.LockedAt = null;
                        seat.UpdatedAt = DateTime.UtcNow;
                    }

                    await _seatRepository.UpdateManyAsync(seats);
                    await _seatRepository.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return ApiResponse<bool>.SuccessResponse(true);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<ApiResponse<bool>> ReleaseBookingSeatsAsync(
            int scheduleId,
            List<string> seatNumbers)
        {
            if (seatNumbers == null || seatNumbers.Count == 0)
                throw new Exception("At least one seat must be specified.");

            using (var transaction = await _scheduleRepository.BeginTransactionAsync())
            {
                try
                {
                    var seats = await _seatRepository.GetSeatsByNumbersAsync(scheduleId, seatNumbers);

                    foreach (var seatNumber in seatNumbers)
                    {
                        var seat = seats.FirstOrDefault(s => s.SeatNumber == seatNumber);

                        if (seat == null)
                            throw new Exception($"Seat {seatNumber} not found.");

                        if (seat.SeatStatus != "Booked")
                            throw new Exception($"Seat {seatNumber} is not booked.");

                        // Convert back to available
                        seat.SeatStatus = "Available";
                        seat.BookingId = null;
                        seat.UpdatedAt = DateTime.UtcNow;
                    }

                    await _seatRepository.UpdateManyAsync(seats);
                    await _seatRepository.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return ApiResponse<bool>.SuccessResponse(true);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
