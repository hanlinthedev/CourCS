using System.Net;
using System.Text.Json;

namespace UserManagement.Middleware
{
   public class ErrorHandlingMiddleware
   {
      private readonly RequestDelegate _next;
      private readonly ILogger<ErrorHandlingMiddleware> _logger;

      public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
         }
      }

      private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
      {
         context.Response.ContentType = "application/json";

         var response = new
         {
            error = "Internal server error.",
            message = exception.Message,
            timestamp = DateTime.UtcNow
         };

         switch (exception)
         {
            case ArgumentException:
               context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
               response = new { error = "Bad request.", message = exception.Message, timestamp = DateTime.UtcNow };
               break;
            case UnauthorizedAccessException:
               context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
               response = new { error = "Unauthorized access.", message = exception.Message, timestamp = DateTime.UtcNow };
               break;
            case KeyNotFoundException:
               context.Response.StatusCode = (int)HttpStatusCode.NotFound;
               response = new { error = "Resource not found.", message = exception.Message, timestamp = DateTime.UtcNow };
               break;
            default:
               context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
               break;
         }

         var jsonResponse = JsonSerializer.Serialize(response);
         await context.Response.WriteAsync(jsonResponse);
      }
   }
}