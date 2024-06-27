using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Security.OpenIddict;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using StoreSettings = VirtoCommerce.StoreModule.Core.ModuleConstants.Settings;

namespace VirtoCommerce.ExperienceApiModule.Core.OpenIddict
{
    public class ContactSignInValidator : ITokenRequestValidator
    {
        public virtual int Priority { get; set; } = 100;

        private readonly IStoreService _storeService;
        private readonly IMemberService _memberService;

        public ContactSignInValidator(IStoreService storeService, IMemberService memberService)
        {
            _storeService = storeService;
            _memberService = memberService;
        }

        public virtual async Task<IList<TokenResponse>> ValidateAsync(TokenRequestContext context)
        {
            // Skip all checks if sign in result is not available
            if (context.SignInResult is null)
            {
                return [];
            }

            // Skip all checks if user is admin
            if (context.User.IsAdministrator)
            {
                return [];
            }

            // Skip all checks if member is not contact
            var contact = await GetContactAsync(context.User);
            if (contact == null)
            {
                return [];
            }

            var store = await GetStore(context);
            if (store == null)
            {
                return [GetError(context.DetailedErrors, ContactSecurityErrorDescriber.UserCannotLoginInStore())];
            }

            if (!context.SignInResult.Succeeded &&
                context.SignInResult.IsLockedOut &&
                contact.Status == "Locked" &&
                !context.User.EmailConfirmed &&
                EmailVerificationRequired(store))
            {
                return [GetError(context.DetailedErrors, ContactSecurityErrorDescriber.EmailVerificationIsRequired())];
            }

            if (context.SignInResult.Succeeded)
            {
                var userStoreId = context.User.StoreId;

                // Allow to sign in to the related stores
                var canLoginToStore =
                    !string.IsNullOrEmpty(userStoreId) && (
                        store.Id == userStoreId ||
                        store.TrustedGroups.Contains(userStoreId));

                if (!canLoginToStore)
                {
                    return [GetError(context.DetailedErrors, ContactSecurityErrorDescriber.UserCannotLoginInStore())];
                }
            }

            return [];
        }

        private async Task<Contact> GetContactAsync(ApplicationUser user)
        {
            return await _memberService.GetByIdAsync(user.MemberId) as Contact;
        }

        private Task<Store> GetStore(TokenRequestContext context)
        {
            var storeId = context.Request.GetParameter(Parameters.StoreId)?.Value?.ToString();

            return string.IsNullOrEmpty(storeId)
                ? null
                : _storeService.GetNoCloneAsync(storeId);
        }

        private static TokenResponse GetError(bool detailedErrors, TokenResponse response)
        {
            return detailedErrors ? response : SecurityErrorDescriber.LoginFailed();
        }

        private static bool EmailVerificationRequired(Store store)
        {
            return store.Settings.GetValue<bool>(StoreSettings.General.EmailVerificationEnabled) &&
                store.Settings.GetValue<bool>(StoreSettings.General.EmailVerificationRequired);
        }
    }
}
