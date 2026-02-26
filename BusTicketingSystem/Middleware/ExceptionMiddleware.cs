using System.Net;
using System.Text.Json;
using BusTicketingSystem.Exceptions;
using BusTicketingSystem.Helpers;

namespace BusTicketingSystem.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next,
                                   ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var message = ex.InnerException?.Message ?? ex.Message;

                var response = ApiResponse<string>.FailureResponse(message);

                await context.Response.WriteAsJsonAsync(response);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;

            if (ex is NotFoundException)
                status = HttpStatusCode.NotFound;
            else if (ex is BadRequestException)
                status = HttpStatusCode.BadRequest;

            var response = ApiResponse<string>.FailureResponse(ex.Message);

            var json = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(json);
        }
    }
}