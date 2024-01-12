using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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

        public virtual async Task<IList<TokenLoginResponse>> ValidateUserAsync(ApplicationUser user, SignInResult signInResult, IDictionary<string, object> context)
        {
            var result = new List<TokenLoginResponse>();

            // skip all check if member is admin
            if (user.IsAdministrator)
            {
                return result;
            }

            // skip all check if member is not contact
            var contact = await GetContactAsync(user);
            if (contact == null)
            {
                return result;
            }

            var detailedErrors = GetDetailedErrors(context);

            var store = await GetStore(context);
            if (store == null)
            {
                var error = detailedErrors ? ContactSecurityErrorDescriber.UserCannotLoginInStore() : SecurityErrorDescriber.LoginFailed();
                result.Add(error);
                return result;
            }

            if (signInResult.IsLockedOut &&
                contact.Status == "Locked" &&
                !user.EmailConfirmed &&
                store.Settings.GetValue<bool>(StoreSettings.General.EmailVerificationEnabled) &&
                store.Settings.GetValue<bool>(StoreSettings.General.EmailVerificationRequired))
            {
                var error = detailedErrors ? ContactSecurityErrorDescriber.EmailVerificationIsRequired() : SecurityErrorDescriber.LoginFailed();
                result.Add(error);
            }

            if (signInResult.Succeeded)
            {
                //Allow to login to store or users not assigned to store
                var canLoginToStore = !user.StoreId.IsNullOrEmpty();

                if (canLoginToStore)
                {
                    canLoginToStore = store.TrustedGroups.Concat(new[] { store.Id }).Contains(user.StoreId);
                }

                if (!canLoginToStore)
                {
                    var error = detailedErrors ? ContactSecurityErrorDescriber.UserCannotLoginInStore() : SecurityErrorDescriber.LoginFailed();
                    result.Add(error);
                }
            }

            return result;
        }

        private async Task<Store> GetStore(IDictionary<string, object> context)
        {
            var store = default(Store);
            if (context.TryGetValue("storeId", out var storeIdValue) && storeIdValue is string storeId)
            {
                store = await _storeService.GetNoCloneAsync(storeId);
            }
            return store;
        }

        private static bool GetDetailedErrors(IDictionary<string, object> context)
        {
            var detailedErrors = false;
            if (context.TryGetValue("detailedErrors", out var detailedErrorsValue))
            {
                detailedErrors = (bool)detailedErrorsValue;
            }
            return detailedErrors;
        }

        private async Task<Contact> GetContactAsync(ApplicationUser user)
        {
            return await _memberService.GetByIdAsync(user.MemberId) as Contact;
        }
    }
}
