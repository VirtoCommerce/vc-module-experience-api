using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Services
{
    public class MemberServiceX : IMemberServiceX
    {
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IServiceProvider _services;


        public MemberServiceX(IMemberService memberService, IMemberSearchService memberSearchService, IAuthorizationService authorizationService, IServiceProvider services)
        {
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _authorizationService = authorizationService;
            _services = services;
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

        public Task<Contact> GetContactByIdAsync(string contactId)
        {
            // TODO: move logic from LoadProfileRequestHandler to here
            throw new NotImplementedException();
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

        public async Task UpdateContactAsync(UserUpdateInfo userUpdateInfo)
        {
            //TODO:Check authorization
            //if (string.IsNullOrEmpty(userUpdateInfo.Id))
            //{
            //    userUpdateInfo.Id = WorkContext.CurrentUser.Id;
            //}
            //var isSelfEditing = userUpdateInfo.Id == WorkContext.CurrentUser.Id;
            var isSelfEditing = true;

            if (!string.IsNullOrEmpty(userUpdateInfo.Id))
            {
                // UserManager<ApplicationUser> requires scoped service
                using (var scope = _services.CreateScope())
                {
                    var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await _userManager.FindByIdAsync(userUpdateInfo.Id);

                    if (user != null)
                    {
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

                        //if (user.Contact != null)
                        //{
                        //    user.Contact.FirstName = userUpdateInfo.FirstName;
                        //    user.Contact.LastName = userUpdateInfo.LastName;
                        //    user.Contact.FullName = userUpdateInfo.FullName;
                        //}

                        user.Email = userUpdateInfo.Email;

                        await _userManager.UpdateAsync(user);
                    }
                }
            }
        }

        public Task UpdateOrganizationAsync(Organization organization)
        {
            throw new NotImplementedException();
        }
    }
}
