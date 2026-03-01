using System;
using System.ComponentModel.DataAnnotations;

namespace BusTicketingSystem.Models
{
    /// <summary>
    /// Stores error logs for monitoring and debugging
    /// </summary>
    public class ErrorLog
    {
        [Key]
        public int ErrorLogId { get; set; }

        /// <summary>
        /// User ID who triggered the error (null if anonymous)
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Exception type name
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string ExceptionType { get; set; }

        /// <summary>
        /// Unique error code
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ErrorCode { get; set; }

        /// <summary>
        /// User-friendly message
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string UserMessage { get; set; }

        /// <summary>
        /// Detailed internal message (may contain sensitive data)
        /// </summary>
        [MaxLength(2000)]
        public string InternalMessage { get; set; }

        /// <summary>
        /// Stack trace for debugging
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Inner exception message if exists
        /// </summary>
        [MaxLength(1000)]
        public string InnerExceptionMessage { get; set; }

        /// <summary>
        /// HTTP Status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Request URL
        /// </summary>
        [MaxLength(500)]
        public string RequestUrl { get; set; }

        /// <summary>
        /// HTTP Method (GET, POST, PUT, DELETE)
        /// </summary>
        [MaxLength(10)]
        public string HttpMethod { get; set; }

        /// <summary>
        /// Request payload
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// Client IP address
        /// </summary>
        [MaxLength(50)]
        public string ClientIpAddress { get; set; }

        /// <summary>
        /// Request headers
        /// </summary>
        public string RequestHeaders { get; set; }

        /// <summary>
        /// Trace ID for request correlation
        /// </summary>
        [MaxLength(100)]
        public string TraceId { get; set; }

        /// <summary>
        /// Validation errors if any
        /// </summary>
        public string ValidationErrors { get; set; }

        /// <summary>
        /// Additional context data
        /// </summary>
        public string ContextData { get; set; }

        /// <summary>
        /// Whether error was handled successfully
        /// </summary>
        public bool IsHandled { get; set; } = true;

        /// <summary>
        /// Whether error is critical and needs immediate attention
        /// </summary>
        public bool IsCritical { get; set; } = false;

        /// <summary>
        /// Error severity level (Info, Warning, Error, Critical)
        /// </summary>
        [MaxLength(20)]
        public string Severity { get; set; } = "Error";

        /// <summary>
        /// When the error occurred
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the error was last reviewed
        /// </summary>
        public DateTime? ResolvedAt { get; set; }

        /// <summary>
        /// Notes about resolution
        /// </summary>
        [MaxLength(500)]
        public string ResolutionNotes { get; set; }
    }
}
