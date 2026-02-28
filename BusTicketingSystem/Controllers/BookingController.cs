using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.Interfaces.Services;
using BusTicketingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusTicketingSystem.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ISeatService _seatService;

        public BookingController(
            IBookingService bookingService,
            ISeatService seatService)
        {
            _bookingService = bookingService;
            _seatService = seatService;
        }

        #region Seat Management Endpoints

        /// <summary>
        /// GET /api/v1/booking/seats/{scheduleId}
        /// Get complete seat layout for a schedule
        /// </summary>
        [Authorize]
        [HttpGet("seats/{scheduleId}")]
        public async Task<IActionResult> GetSeatLayout(int scheduleId)
        {
            try
            {
                var result = await _seatService.GetSeatLayoutAsync(scheduleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// POST /api/v1/booking/seats/lock
        /// Lock selected seats for 5 minutes
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpPost("seats/lock")]
        public async Task<IActionResult> LockSeats([FromBody] LockSeatsRequestDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid token. UserId claim missing.",
                        Data = null
                    });
                }

                int userId = int.Parse(userIdClaim.Value);
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

                var result = await _seatService.LockSeatsAsync(
                    dto.ScheduleId,
                    dto.SeatNumbers,
                    userId,
                    ip);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// POST /api/v1/booking/seats/release
        /// Release locked seats
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpPost("seats/release")]
        public async Task<IActionResult> ReleaseSeats([FromBody] ReleaseSeatsRequestDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid token. UserId claim missing.",
                        Data = null
                    });
                }

                int userId = int.Parse(userIdClaim.Value);
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

                var result = await _seatService.ReleaseSeatsAsync(
                    dto.ScheduleId,
                    dto.SeatNumbers,
                    userId,
                    ip);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        #endregion

        #region Booking Endpoints

        /// <summary>
        /// POST /api/v1/booking
        /// Create booking with previously locked seats
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingRequestDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid token. UserId claim missing.",
                        Data = null
                    });
                }

                int userId = int.Parse(userIdClaim.Value);
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

                return Ok(await _bookingService
                    .CreateBookingAsync(dto, userId, ip));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// GET /api/v1/booking/my
        /// Get current user's bookings
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpGet("my")]
        public async Task<IActionResult> MyBookings()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid token. UserId claim missing.",
                        Data = null
                    });
                }

                int userId = int.Parse(userIdClaim.Value);

                return Ok(await _bookingService
                    .GetMyBookingsAsync(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// GET /api/v1/booking
        /// Get all bookings (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AllBookings()
        {
            try
            {
                return Ok(await _bookingService
                    .GetAllBookingsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// GET /api/v1/booking/{id}
        /// Get booking details by ID
        /// </summary>
        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            try
            {
                return Ok(await _bookingService
                    .GetBookingByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// PUT /api/v1/booking/cancel/{id}
        /// Cancel booking and release seats
        /// </summary>
        [Authorize(Roles = "Admin,Customer")]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid token. UserId claim missing.",
                        Data = null
                    });
                }

                int userId = int.Parse(userIdClaim.Value);

                string role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

                return Ok(await _bookingService
                    .CancelBookingAsync(id, userId, role, ip));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        #endregion
    }
}
