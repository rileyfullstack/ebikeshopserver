using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ebikeshopserver.Utils
{
    public static class UserIdProvider
    {
        public static string? GetUserId(HttpContext httpContext)
        {
            return httpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        }
    }
}
