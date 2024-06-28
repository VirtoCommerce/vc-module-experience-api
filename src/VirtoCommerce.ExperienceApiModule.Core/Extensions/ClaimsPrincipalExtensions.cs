using System.Security.Claims;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetCurrentUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? claimsPrincipal?.FindFirstValue("name") ?? AnonymousUser.UserName;
    }
}
