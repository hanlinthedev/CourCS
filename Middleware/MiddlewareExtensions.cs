namespace UserManagement.Middleware
{
   public static class MiddlewareExtensions
   {
      public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
      {
         return builder.UseMiddleware<ErrorHandlingMiddleware>();
      }

      public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
      {
         return builder.UseMiddleware<AuthenticationMiddleware>();
      }

      public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
      {
         return builder.UseMiddleware<RequestLoggingMiddleware>();
      }
   }
}