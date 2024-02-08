using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ebikeshopserver.Utils; // Assuming this is where UserIdProvider is located

namespace ebikeshopserver.Authorization
{
    public static class AuthorizationHelper
    {
        public static (bool IsAuthorized, IActionResult? ActionResult) AuthorizeUserOrAdmin(HttpContext httpContext, string requestedUserId)
        {
            var currentUserId = UserIdProvider.GetUserId(httpContext);
            if (currentUserId == null)
            {
                return (false, new UnauthorizedObjectResult("User is not authenticated."));
            }

            var isAdmin = httpContext.User.IsInRole("Admin");
            if (!isAdmin && currentUserId != requestedUserId)
            {
                return (false, new ForbidResult("You are not authorized to view this information."));
            }

            return (true, null);
        }
    }
}
