using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.DTOs.Responses;
using BusTicketingSystem.Helpers;
using BusTicketingSystem.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusTicketingSystem.Controllers
{
    [ApiController]
    [Route("api/v1/buses")]
    public class BusesController : ControllerBase
    {
        private readonly IBusService _busService;

        public BusesController(IBusService busService)
        {
            _busService = busService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBus(CreateBusRequest request)
        {
            //var result = await _busService.CreateBusAsync(request);

            //var userIdClaim = User.FindFirst("UserId");

            //if (userIdClaim == null)
            //    throw new UnauthorizedAccessException("Invalid token.");

            //var userId = int.Parse(userIdClaim.Value);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var result = await _busService.CreateBusAsync(request, userId, ipAddress);

            return Ok(ApiResponse<BusResponse>.SuccessResponse("Bus created successfully", result));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllBuses(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _busService.GetAllBusesAsync(pageNumber, pageSize);
            return Ok(ApiResponse<List<BusResponse>>.SuccessResponse(result));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBus(int id)
        {
            var result = await _busService.GetBusByIdAsync(id);
            return Ok(ApiResponse<BusResponse>.SuccessResponse(result));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBus(int id, [FromBody] UpdateBusRequest request)
        {
            if (request == null)
                return BadRequest(ApiResponse<string>.FailureResponse("Invalid request body."));

            var userIdClaim = User.FindFirst("UserId")
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(ApiResponse<string>.FailureResponse("Invalid token."));

            var userId = int.Parse(userIdClaim.Value);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _busService.UpdateBusAsync(id, request, userId, ipAddress);

            return Ok(ApiResponse<string>.SuccessResponse("Bus updated successfully."));
        }
        

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            await _busService.DeleteBusAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Bus deleted successfully"));
        }


        [AllowAnonymous]
        [HttpGet("by-operator")]
        public async Task<IActionResult> GetByOperator(
            string operatorName,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var (buses, totalCount) =
                await _busService.GetByOperatorAsync(operatorName, pageNumber, pageSize);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                totalCount,
                pageNumber,
                pageSize,
                data = buses
            }));
        }
    }
}