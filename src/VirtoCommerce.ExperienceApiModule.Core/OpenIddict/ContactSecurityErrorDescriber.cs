using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Security.OpenIddict;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace VirtoCommerce.ExperienceApiModule.Core.OpenIddict
{
    public static class ContactSecurityErrorDescriber
    {
        public static TokenResponse UserCannotLoginInStore() => new()
        {
            Error = Errors.InvalidGrant,
            Code = nameof(UserCannotLoginInStore).ToSnakeCase(),
            ErrorDescription = "Access denied. You cannot sign in to the current store"
        };

        public static TokenResponse EmailVerificationIsRequired() => new()
        {
            Error = Errors.InvalidGrant,
            Code = nameof(EmailVerificationIsRequired).ToSnakeCase(),
            ErrorDescription = "Email verification required. Please verify your email address."
        };

        public static TokenResponse InvalidOrganizationId(string organizationId) => new()
        {
            Error = Errors.InvalidGrant,
            Code = nameof(InvalidOrganizationId).ToSnakeCase(),
            ErrorDescription = $"Cannot switch to organization '{organizationId}'."
        };
    }
}
