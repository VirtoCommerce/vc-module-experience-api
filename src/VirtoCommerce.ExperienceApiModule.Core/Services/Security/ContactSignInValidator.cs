using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Security;
using VirtoCommerce.Platform.Security.Model;
using VirtoCommerce.Platform.Security.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using StoreSettings = VirtoCommerce.StoreModule.Core.ModuleConstants.Settings;

namespace VirtoCommerce.ExperienceApiModule.Core.Services.Security
{
    public class ContactSignInValidator : IUserSignInValidator
    {
        public virtual int Priority { get; set; } = 100;

        private readonly IStoreService _storeService;
        private readonly IMemberService _memberService;

        public ContactSignInValidator(IStoreService storeService, IMemberService memberService)
        {
            _storeService = storeService;
            _memberService = memberService;
        }

        public virtual async Task<IList<TokenLoginResponse>> ValidateUserAsync(SignInValidatorContext context)
        {
            var result = new List<TokenLoginResponse>();

            // skip all check if member is admin
            if (context.User.IsAdministrator)
            {
                return result;
            }

            // skip all check if member is not contact
            var contact = await GetContactAsync(context.User);
            if (contact == null)
            {
                return result;
            }

            var store = await _storeService.GetNoCloneAsync(context.StoreId);
            if (store == null)
            {
                var error = GetError(context.DetailedErrors, ContactSecurityErrorDescriber.UserCannotLoginInStore());
                result.Add(error);
                return result;
            }

            if (!context.IsSucceeded &&
                context.IsLockedOut &&
                contact.Status == "Locked" &&
                !context.User.EmailConfirmed &&
                EmailVerificationRequired(store))
            {
                var error = GetError(context.DetailedErrors, ContactSecurityErrorDescriber.EmailVerificationIsRequired());
                result.Add(error);
            }

            if (context.IsSucceeded)
            {
                //Allow to login to store or users not assigned to store
                var canLoginToStore = !context.User.StoreId.IsNullOrEmpty();

                if (canLoginToStore)
                {
                    canLoginToStore = store.TrustedGroups.Concat(new[] { store.Id }).Contains(context.User.StoreId);
                }

                if (!canLoginToStore)
                {
                    var error = GetError(context.DetailedErrors, ContactSecurityErrorDescriber.UserCannotLoginInStore());
                    result.Add(error);
                }
            }

            return result;
        }

        private async Task<Contact> GetContactAsync(ApplicationUser user)
        {
            return await _memberService.GetByIdAsync(user.MemberId) as Contact;
        }

        private static TokenLoginResponse GetError(bool detailedErrors, TokenLoginResponse response)
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
