using System.Text.RegularExpressions;

namespace UserManagement.Utils
{
   public static class Utils
   {
      public static string ValidateUser(User user)
      {
         if (string.IsNullOrWhiteSpace(user.Name))
            return "Name is required.";

         if (string.IsNullOrWhiteSpace(user.Email))
            return "Email is required.";

         if (!IsValidEmail(user.Email))
            return "Invalid email format.";

         if (user.Age < 0 || user.Age > 150)
            return "Age must be between 0 and 150.";

         return string.Empty;
      }

      private static bool IsValidEmail(string email)
      {
         var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
         return emailRegex.IsMatch(email);
      }
   }
}