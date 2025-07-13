namespace UserManagement.Middleware
{
   public class AuthenticationMiddleware
   {
      private readonly RequestDelegate _next;
      private readonly ILogger<AuthenticationMiddleware> _logger;
      private readonly HashSet<string> _validTokens;

      public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
      {
         _next = next;
         _logger = logger;

         // In production, tokens would be validated against a secure token service
         _validTokens = new HashSet<string>
            {
                "valid-token-123",
                "admin-token-456",
                "user-token-789"
            };
      }

      public async Task InvokeAsync(HttpContext context)
      {
         // Skip authentication for Swagger endpoints in development
         if (context.Request.Path.StartsWithSegments("/swagger") ||
             context.Request.Path.StartsWithSegments("/favicon.ico"))
         {
            await _next(context);
            return;
         }

         // Extract token from Authorization header
         var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

         if (authHeader == null || !authHeader.StartsWith("Bearer "))
         {
            _logger.LogWarning("Missing or invalid Authorization header for {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized. Token required." });
            return;
         }

         var token = authHeader.Substring("Bearer ".Length).Trim();

         if (!_validTokens.Contains(token))
         {
            _logger.LogWarning("Invalid token provided for {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized. Invalid token." });
            return;
         }

         _logger.LogInformation("Valid token provided for {Path}", context.Request.Path);
         await _next(context);
      }
   }
}