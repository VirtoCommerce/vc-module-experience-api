using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Services
{
    public class MemberServiceX : IMemberServiceX
    {
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;
        private readonly ICustomerOrderSearchService _orderSearchService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IServiceProvider _services;
        private readonly IMapper _mapper;


        public MemberServiceX(IMemberService memberService, IMemberSearchService memberSearchService, ICustomerOrderSearchService orderSearchService, IAuthorizationService authorizationService, IServiceProvider services, IMapper mapper)
        {
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _orderSearchService = orderSearchService;
            _authorizationService = authorizationService;
            _services = services;
            _mapper = mapper;
        }

        public Task<Contact> CreateContactAsync(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Task<Organization> CreateOrganizationAsync(Organization organization)
        {
            throw new NotImplementedException();
        }

        public Task DeleteContactAsync(string contactId)
        {
            throw new NotImplementedException();
        }

        public async Task<Profile> GetProfileByIdAsync(string userId)
        {
            var user = await FindUserByIdAsync(userId);

            if (user != null)
            {
                var profile = new Profile();
                profile.User = user;

                if (user.MemberId != null)
                {
                    profile.Contact = await _memberService.GetByIdAsync(user.MemberId, null, nameof(Contact)) as Contact;
                }

                return profile;
            }

            return default;
        }

        public Task<Organization> GetOrganizationByIdAsync(string organizationId)
        {
            throw new NotImplementedException();
        }

        public async Task<Contact> UpdateContactAddressesAsync(string contactId, IList<Address> addresses)
        {
            var member = await _memberService.GetByIdAsync(contactId);
            if (member != null)
            {
                //if (!(await AuthorizeAsync(member, ModuleConstants.Security.Permissions.Update)).Succeeded)
                //{
                //    return Unauthorized();
                //}
                member.Addresses = addresses.ToList();
                await _memberService.SaveChangesAsync(new[] { member });

                return await _memberService.GetByIdAsync(contactId, null, nameof(Contact)) as Contact;
            }

            return default;
        }

        public async Task<Profile> UpdateContactAsync(UserUpdateInfo userUpdateInfo)
        {
            if (!string.IsNullOrEmpty(userUpdateInfo.Id))
            {
                // UserManager<ApplicationUser> requires scoped service
                using (var scope = _services.CreateScope())
                {
                    var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await _userManager.FindByIdAsync(userUpdateInfo.Id);

                    if (user != null)
                    {
                        //TODO:Check authorization
                        //if (string.IsNullOrEmpty(userUpdateInfo.Id))
                        //{
                        //    userUpdateInfo.Id = WorkContext.CurrentUser.Id;
                        //}
                        //var isSelfEditing = userUpdateInfo.Id == WorkContext.CurrentUser.Id;
                        var isSelfEditing = true;
                        if (!isSelfEditing)
                        {
                            //var authorizationResult = await _authorizationService.AuthorizeAsync(User, null, SecurityConstants.Permissions.CanEditUsers);
                            //if (authorizationResult.Succeeded)
                            //{
                            //    authorizationResult = await _authorizationService.AuthorizeAsync(User, user?.Contact?.Organization, CanEditOrganizationResourceAuthorizeRequirement.PolicyName);
                            //}
                            //if (!authorizationResult.Succeeded)
                            //{
                            //    return Unauthorized();
                            //}

                            //Don't allow change self roles
                            user.Roles = userUpdateInfo.Roles?.Select(x => new Role { Id = x, Name = x }).ToList();
                        }

                        if (!user.MemberId.IsNullOrEmpty())
                        {
                            var member = await _memberService.GetByIdAsync(user.MemberId);
                            if (member != null)
                            {
                                _mapper.Map(userUpdateInfo, member);
                                member.Id = user.MemberId;

                                await _memberService.SaveChangesAsync(new[] { member });
                            }
                        }

                        _mapper.Map(userUpdateInfo, user);

                        await _userManager.UpdateAsync(user);

                        return await GetProfileByIdAsync(userUpdateInfo.Id);
                    }
                }
            }

            return default;
        }

        public async Task<IdentityResult> UpdatePhoneNumberAsync(PhoneNumberUpdateInfo updateInfo)
        {
            // UserManager<ApplicationUser> requires scoped service
            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await _userManager.FindByIdAsync(updateInfo.Id);

                if (user != null)
                {
                    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, updateInfo.PhoneNumber);
                    return await _userManager.ChangePhoneNumberAsync(user, updateInfo.PhoneNumber, code);
                }
            }

            return default;
        }

        public async Task<IdentityResult> RemovePhoneNumberAsync(string userId)
        {
            // UserManager<ApplicationUser> requires scoped service
            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    var twoFactorAuthEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                    if (twoFactorAuthEnabled)
                    {
                        return IdentityResult.Failed(new[] { new IdentityError { Description = "Can't remove while two factor authentication enabled." } });
                    }

                    return await _userManager.SetPhoneNumberAsync(user, null);
                }
            }

            return IdentityResult.Failed(new[] { new IdentityError { Description = "User not found." } });
        }

        private async Task<ApplicationUser> FindUserByIdAsync(string id)
        {
            ApplicationUser result;

            // UserManager<ApplicationUser> requires scoped service
            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                result = await _userManager.FindByIdAsync(id);
            }

            return result;
        }

        public async Task<Organization> UpdateOrganizationAsync(OrganizationUpdateInfo organizationUpdateInfo)
        {
            //Allow to register new users only within own organization
            //var authorizationResult = await _authorizationService.AuthorizeAsync(User, organization, CanEditOrganizationResourceAuthorizeRequirement.PolicyName);
            //if (!authorizationResult.Succeeded)
            //{
            //    return Unauthorized();
            //}
            var member = await _memberService.GetByIdAsync(organizationUpdateInfo.Id, null, nameof(Organization)) as Organization;
            if (member != null)
            {
                _mapper.Map(organizationUpdateInfo, member);

                return await UpdateOrganizationAsync(member);
            }

            return default;
        }

        public async Task<Organization> UpdateOrganizationAsync(Organization org)
        {
            await _memberService.SaveChangesAsync(new[] { org });

            return await _memberService.GetByIdAsync(org.Id, null, nameof(Organization)) as Organization;
        }

        public async Task<MemberSearchResult> SearchOrganizationContactsAsync(MembersSearchCriteria criteria)
        {
            return await _memberSearchService.SearchMembersAsync(criteria);
        }

        public async Task<ProfileSearchResult> SearchOrganizationProfilesAsync(MembersSearchCriteria criteria)
        {
            var result = AbstractTypeFactory<ProfileSearchResult>.TryCreateInstance();
            var searchResult = await _memberSearchService.SearchMembersAsync(criteria);
            var profiles = await Task.WhenAll(searchResult.Results
                        .OfType<Contact>()
                        .Where(x => x.SecurityAccounts.Any())
                        .Select(x => GetProfileByIdAsync(x.SecurityAccounts.FirstOrDefault().Id))
                        );

            result.Results = profiles;
            result.TotalCount = searchResult.TotalCount;

            return result;
        }

        public async Task<IdentityResult> LockUserAsync(string userId)
        {
            // UserManager<ApplicationUser> requires scoped service
            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    // TODO: auth.
                    ////Allow to register new users only within own organization
                    //var authorizationResult = await _authorizationService.AuthorizeAsync(User, user?.Contact?.Organization, CanEditOrganizationResourceAuthorizeRequirement.PolicyName);
                    //if (!authorizationResult.Succeeded)
                    //{
                    //    return Unauthorized();
                    //}

                    return await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                }
            }

            return IdentityResult.Failed(new[] { new IdentityError { Description = "User not found." } });
        }

        public async Task<IdentityResult> UnlockUserAsync(string userId)
        {
            // UserManager<ApplicationUser> requires scoped service
            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    // TODO: auth.
                    ////Allow to register new users only within own organization
                    //var authorizationResult = await _authorizationService.AuthorizeAsync(User, user?.Contact?.Organization, CanEditOrganizationResourceAuthorizeRequirement.PolicyName);
                    //if (!authorizationResult.Succeeded)
                    //{
                    //    return Unauthorized();
                    //}

                    await _userManager.ResetAccessFailedCountAsync(user);
                    return await _userManager.SetLockoutEndDateAsync(user, null);
                }
            }

            return IdentityResult.Failed(new[] { new IdentityError { Description = "User not found." } });
        }
    }
}
