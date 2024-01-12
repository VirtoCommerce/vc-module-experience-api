using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Security.Model;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace VirtoCommerce.ExperienceApiModule.Core.Services.Security
{
    public static class ContactSecurityErrorDescriber
    {
        public static TokenLoginResponse UserCannotLoginInStore() => new()
        {
            Error = Errors.InvalidGrant,
            Code = nameof(UserCannotLoginInStore).PascalToKebabCase(),
            ErrorDescription = "Access denied. You cannot sign in to the current store"
        };

        public static TokenLoginResponse EmailVerificationIsRequired() => new()
        {
            Error = Errors.InvalidGrant,
            Code = nameof(EmailVerificationIsRequired).PascalToKebabCase(),
            ErrorDescription = "Email verification required. Please verify your email address."
        };
    }
}
