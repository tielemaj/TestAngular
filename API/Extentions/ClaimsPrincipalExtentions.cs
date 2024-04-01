using System.Security.Claims;

namespace API.Extentions
{
    public static class ClaimsPrincipalExtentions
    {
        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}