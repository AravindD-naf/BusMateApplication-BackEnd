using Asp.Versioning;
using BusTicketingSystem.Helpers;
using BusTicketingSystem.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingSystem.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auditlogs")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditLogsController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs(
            int pageNumber = 1,
            int pageSize = 10,
            string? entityName = null,
            int? userId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var (logs, totalCount) = await _auditService.GetPagedLogsAsync(
                pageNumber, pageSize, entityName, userId, fromDate, toDate);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                totalCount,
                pageNumber,
                pageSize,
                data = logs
            }));
        }
    }
}