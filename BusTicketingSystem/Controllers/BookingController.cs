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
        private readonly IPaymentService _paymentService;
        private readonly IPassengerService _passengerService;
        private readonly IScheduleService _scheduleService;

        public BookingController(
            IBookingService bookingService,
            ISeatService seatService,
            IPaymentService paymentService,
            IPassengerService passengerService,
            IScheduleService scheduleService)
        {
            _bookingService = bookingService;
            _seatService = seatService;
            _paymentService = paymentService;
            _passengerService = passengerService;
            _scheduleService = scheduleService;
        }

        #region Schedule Browsing Endpoints

        /// <summary>
        /// GET /api/v1/booking/schedules
        /// Get all available schedules for browsing (Public)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("schedules")]
        public async Task<IActionResult> GetSchedules(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _scheduleService.GetAllAsync(pageNumber, pageSize);
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
        /// GET /api/v1/booking/schedules/search
        /// Search schedules by route and date (Public)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("schedules/search")]
        public async Task<IActionResult> SearchSchedules(
            [FromQuery] string fromCity,
            [FromQuery] string toCity,
            [FromQuery] DateTime travelDate)
        {
            try
            {
                var result = await _scheduleService
                    .SearchSchedulesAsync(fromCity, toCity, travelDate);
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

        #region Payment Endpoints

        /// <summary>
        /// POST /api/v1/booking/payment/initiate
        /// Initiate payment for a booking
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpPost("payment/initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequestDto dto)
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

                var result = await _paymentService.InitiatePaymentAsync(
                    dto.BookingId,
                    dto.Amount,
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
        /// POST /api/v1/booking/payment/confirm
        /// Confirm payment (dummy payment processing)
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpPost("payment/confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequestDto dto)
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

                var result = await _paymentService.ConfirmPaymentAsync(dto, userId, ip);

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
        /// GET /api/v1/booking/payment/{paymentId}
        /// Get payment details
        /// </summary>
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("payment/{paymentId}")]
        public async Task<IActionResult> GetPayment(int paymentId)
        {
            try
            {
                var result = await _paymentService.GetPaymentAsync(paymentId);
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

        #region Passenger Endpoints

        /// <summary>
        /// POST /api/v1/booking/passengers
        /// Add passenger details to booking
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpPost("passengers")]
        public async Task<IActionResult> AddPassengers([FromBody] AddPassengerRequestDto dto)
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

                var result = await _passengerService.AddPassengersAsync(dto, userId, ip);

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
        /// GET /api/v1/booking/{bookingId}/passengers
        /// Get all passengers for a booking
        /// </summary>
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("{bookingId}/passengers")]
        public async Task<IActionResult> GetPassengers(int bookingId)
        {
            try
            {
                var result = await _passengerService.GetBookingPassengersAsync(bookingId);
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

        #region Refund Endpoints

        /// <summary>
        /// POST /api/v1/booking/refund/confirm
        /// Confirm refund (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("refund/confirm")]
        public async Task<IActionResult> ConfirmRefund([FromBody] ConfirmRefundRequestDto dto)
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

                var result = await _paymentService.ConfirmRefundAsync(dto, userId, ip);

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
        /// GET /api/v1/booking/refund/{refundId}
        /// Get refund details
        /// </summary>
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("refund/{refundId}")]
        public async Task<IActionResult> GetRefund(int refundId)
        {
            try
            {
                var result = await _paymentService.GetRefundAsync(refundId);
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
    }
}
