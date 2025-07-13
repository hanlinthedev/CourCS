using System.Diagnostics;

namespace UserManagement.Middleware
{
   public class RequestLoggingMiddleware
   {
      private readonly RequestDelegate _next;
      private readonly ILogger<RequestLoggingMiddleware> _logger;

      public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
      {
         _next = next;
         _logger = logger;
      }

      public async Task InvokeAsync(HttpContext context)
      {
         var stopwatch = Stopwatch.StartNew();

         // Log incoming request
         _logger.LogInformation("Incoming Request: {Method} {Path} at {Timestamp}",
             context.Request.Method,
             context.Request.Path,
             DateTime.UtcNow);

         // Continue to next middleware
         await _next(context);

         stopwatch.Stop();

         // Log outgoing response
         _logger.LogInformation("Outgoing Response: {Method} {Path} responded with {StatusCode} in {ElapsedMilliseconds}ms",
             context.Request.Method,
             context.Request.Path,
             context.Response.StatusCode,
             stopwatch.ElapsedMilliseconds);
      }
   }
}