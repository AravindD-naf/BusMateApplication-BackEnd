using System.Collections.Generic;

namespace BusTicketingSystem.Exceptions
{
    /// <summary>
    /// Base exception for all application-specific exceptions
    /// Provides error codes, status codes, and user-friendly messages
    /// </summary>
    public class ApplicationException : Exception
    {
        /// <summary>
        /// Unique error code for identification (e.g., "VAL001", "NOT_FOUND_001")
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// HTTP status code to return (e.g., 400, 404, 409, 500)
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// User-friendly message safe to expose to clients
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Internal detailed message for logging (may contain sensitive info)
        /// </summary>
        public string InternalMessage { get; set; }

        /// <summary>
        /// Validation error details (key-value pairs)
        /// </summary>
        public Dictionary<string, string> Errors { get; set; }

        /// <summary>
        /// Additional context data for debugging
        /// </summary>
        public Dictionary<string, object> ContextData { get; set; }

        /// <summary>
        /// Timestamp when exception occurred
        /// </summary>
        public DateTime Timestamp { get; set; }

        public ApplicationException(
            string userMessage,
            string errorCode = "ERROR_001",
            int statusCode = 500,
            string internalMessage = null,
            Exception innerException = null)
            : base(userMessage, innerException)
        {
            UserMessage = userMessage;
            ErrorCode = errorCode;
            StatusCode = statusCode;
            InternalMessage = internalMessage ?? userMessage;
            Errors = new Dictionary<string, string>();
            ContextData = new Dictionary<string, object>();
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Add validation error
        /// </summary>
        public ApplicationException AddError(string field, string message)
        {
            Errors[field] = message;
            return this;
        }

        /// <summary>
        /// Add context data for debugging
        /// </summary>
        public ApplicationException AddContextData(string key, object value)
        {
            ContextData[key] = value;
            return this;
        }
    }
}
