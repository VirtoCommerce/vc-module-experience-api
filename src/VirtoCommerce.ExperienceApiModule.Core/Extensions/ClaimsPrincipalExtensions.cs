using System.Security.Claims;
using VirtoCommerce.ExperienceApiModule.Core.OpenIddict;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetCurrentUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? claimsPrincipal?.FindFirstValue("name") ?? AnonymousUser.UserName;
    }

    public static string GetCurrentOrganizationId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal?.FindFirstValue(Claims.OrganizationId);
    }
}
