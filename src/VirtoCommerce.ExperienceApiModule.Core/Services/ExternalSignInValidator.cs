using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security.ExternalSignIn;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Core.Services;

public class ExternalSignInValidator(
    IStoreService storeService,
    IStoreAuthenticationService storeAuthenticationService)
    : IExternalSignInValidator
{
    public async Task<bool> ValidateAsync(ExternalSignInRequest request)
    {
        var store = await storeService.GetNoCloneAsync(request.StoreId);

        if (store is null ||
            string.IsNullOrEmpty(store.Url) ||
            !Uri.TryCreate(store.Url, UriKind.Absolute, out var storeUri) ||
            !IsValidUrl(storeUri, request.OidcUrl) ||
            !IsValidUrl(storeUri, request.CallbackUrl))
        {
            return false;
        }

        var schemes = await storeAuthenticationService.GetStoreSchemesAsync(request.StoreId, clone: false);

        return schemes.Any(x => x.IsActive && x.Name.EqualsIgnoreCase(request.AuthenticationType));
    }

    private static bool IsValidUrl(Uri baseUri, string uriString)
    {
        return Uri.TryCreate(uriString, UriKind.Absolute, out var uri) && baseUri.IsBaseOf(uri);
    }
}
