using Asp.Versioning;
using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.DTOs.Responses;
using BusTicketingSystem.Interfaces.Services;
using BusTicketingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusTicketingSystem.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/routes")]
    [Authorize]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        // 🔹 GET: All Routes (Admin + Customer allowed)
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _routeService.GetAllRoutesAsync(pageNumber, pageSize);
            return Ok(response);
        }

        // 🔹 GET: Route By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _routeService.GetRouteByIdAsync(id);
            return Ok(response);
        }

        // 🔒 POST: Create Route (Admin Only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RouteCreateRequestDto request)
        {
            var userId = GetUserId();
            var ipAddress = GetIpAddress();

            var response = await _routeService
                .CreateRouteAsync(request, userId, ipAddress);

            return Ok(response);
        }

        // 🔒 PUT: Update Route (Admin Only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, RouteUpdateRequestDto request)
        {
            var userId = GetUserId();
            var ipAddress = GetIpAddress();

            var response = await _routeService
                .UpdateRouteAsync(id, request, userId, ipAddress);

            return Ok(response);
        }

        // 🔒 DELETE: Soft Delete Route (Admin Only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var ipAddress = GetIpAddress();

            var response = await _routeService
                .DeleteRouteAsync(id, userId, ipAddress);

            return Ok(response);
        }

        // 🔹 GET: Routes by Source
        [HttpGet("by-source")]
        public async Task<IActionResult> GetBySource(
            [FromQuery] string source,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var response = await _routeService
                .GetRoutesBySourceAsync(source, pageNumber, pageSize);

            return Ok(response);
        }

        // 🔹 GET: Routes by Destination
        [HttpGet("by-destination")]
        public async Task<IActionResult> GetByDestination(
            [FromQuery] string destination,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var response = await _routeService
                .GetRoutesByDestinationAsync(destination, pageNumber, pageSize);

            return Ok(response);
        }

        // 🔍 Dynamic Route Search
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? source,
            [FromQuery] string? destination,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var response = await _routeService
                .SearchRoutesAsync(source, destination, pageNumber, pageSize);

            return Ok(response);
        }

        // -----------------------
        // 🔧 Helper Methods
        // -----------------------

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return claim != null ? int.Parse(claim) : 0;
        }

        private string GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}