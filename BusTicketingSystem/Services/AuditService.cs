using BusTicketingSystem.DTOs.Responses;
using BusTicketingSystem.Interfaces.Repositories;
using BusTicketingSystem.Interfaces.Services;
using BusTicketingSystem.Models;
using System.Text.Json;

namespace BusTicketingSystem.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;

        public AuditService(IAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        public async Task LogAsync(
            int? userId,
            string action,
            string entityName,
            string? entityId,
            object? oldValues,
            object? newValues,
            string? ipAddress)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                OldValues = oldValues != null
                    ? JsonSerializer.Serialize(oldValues)
                    : null,
                NewValues = newValues != null
                    ? JsonSerializer.Serialize(newValues)
                    : null,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            await _auditRepository.AddAsync(log);
        }

        public async Task<(List<AuditLogResponse>, int totalCount)> GetPagedLogsAsync(
            int pageNumber,
            int pageSize,
            string? entityName,
            int? userId,
            DateTime? fromDate,
            DateTime? toDate
        )

        {
            var (logs, totalCount) = await _auditRepository
                .GetPagedAsync(pageNumber, pageSize, entityName, userId, fromDate, toDate);

            var response = logs.Select(a => new AuditLogResponse
            {
                AuditId = a.AuditId,
                UserId = a.UserId,
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                IpAddress = a.IpAddress,
                Timestamp = a.Timestamp
            }).ToList();

            return (response, totalCount);
        }
    }
}