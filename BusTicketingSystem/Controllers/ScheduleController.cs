//using BusTicketingSystem.DTOs;
//using BusTicketingSystem.Interfaces.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//[ApiController]
//[Route("api/v1/[controller]")]
//[Authorize(Roles = "Admin")]
//public class ScheduleController : ControllerBase
//{
//    private readonly IScheduleService _scheduleService;

//    public ScheduleController(IScheduleService scheduleService)
//    {
//        _scheduleService = scheduleService;
//    }

//    [HttpPost]
//    public async Task<IActionResult> Create(ScheduleRequestDto dto)
//    {
//        var response = await _scheduleService
//            .CreateAsync(dto, 1, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

//        return Ok(response);
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
//    {
//        var response = await _scheduleService
//            .GetAllAsync(pageNumber, pageSize);

//        return Ok(response);
//    }

//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        var response = await _scheduleService.GetByIdAsync(id);
//        return Ok(response);
//    }
//}







using BusTicketingSystem.DTOs;
using BusTicketingSystem.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingSystem.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // ✅ 1. CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScheduleRequestDto dto)
        {
            var response = await _scheduleService.CreateAsync(
                dto,
                1,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

            return Ok(response);
        }

        // ✅ 2. GET ALL (Paginated)
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _scheduleService
                .GetAllAsync(pageNumber, pageSize);

            return Ok(response);
        }

        // ✅ 3. GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _scheduleService.GetByIdAsync(id);
            return Ok(response);
        }

        // ✅ 4. UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ScheduleRequestDto dto)
        {
            var response = await _scheduleService.UpdateAsync(
                id,
                dto,
                1,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

            return Ok(response);
        }

        // ✅ 5. DELETE (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _scheduleService.DeleteAsync(
                id,
                1,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

            return Ok(response);
        }


        [HttpGet("from/{fromCity}")]
        public async Task<IActionResult> GetByFromCity(string fromCity)
        {
            var response = await _scheduleService.GetByFromCityAsync(fromCity);
            return Ok(response);
        }

        [HttpGet("to/{toCity}")]
        public async Task<IActionResult> GetByToCity(string toCity)
        {
            var response = await _scheduleService.GetByToCityAsync(toCity);
            return Ok(response);
        }

        [HttpGet("search")] 
        public async Task<IActionResult> Search(
            [FromQuery] string fromCity,
            [FromQuery] string toCity,
            [FromQuery] DateTime travelDate)
        {
            var response = await _scheduleService
                .SearchSchedulesAsync(fromCity, toCity, travelDate);

            return Ok(response);
        }
    }
}