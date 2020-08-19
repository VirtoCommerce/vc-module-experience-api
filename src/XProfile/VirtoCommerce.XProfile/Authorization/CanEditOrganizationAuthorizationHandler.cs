using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Authorization
{
    public class CanEditOrganizationAuthorizationRequirement : PermissionAuthorizationRequirement
    {
        public CanEditOrganizationAuthorizationRequirement() : base("CanEditOrganization")
        {
        }
    }

    public class CanEditOrganizationAuthorizationHandler : PermissionAuthorizationHandlerBase<CanEditOrganizationAuthorizationRequirement>
    {
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;
        private readonly UserManager<ApplicationUser> _userManager;


        public CanEditOrganizationAuthorizationHandler(IMemberService memberService, IMemberSearchService memberSearchService, Func<UserManager<ApplicationUser>> userManager)
        {
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _userManager = userManager();
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEditOrganizationAuthorizationRequirement requirement)
        {

            var currentContact = await GetCustomerAsync(GetUserId(context)) as Contact;

            var result = false;
            if (context.Resource is ContactAggregate contactAggregate)
            {
                result = contactAggregate.Contact.Id == GetUserId(context);
            }
            if (context.Resource is OrganizationAggregate organizationAggregate && currentContact != null)
            {
                result = currentContact.Organizations.Contains(organizationAggregate.Organization.Name);
            }
            else if (context.Resource is CreateContactCommand createContactCommand && currentContact != null)
            {
                result = currentContact.Organizations.Intersect(createContactCommand.Organizations).Any();
            }
            else if (context.Resource is CreateOrganizationCommand createOrganizationCommand)
            {
                //Can be checked only with platform permission
                result = true;
            }
            else if (context.Resource is CreateUserCommand createUserCommand)
            {
                //Can be checked only with platform permission
                result = true;
            }
            else if (context.Resource is DeleteContactCommand deleteContactCommand && currentContact != null)
            {
                result = await HasSameOrganizationAsync(currentContact, deleteContactCommand.ContactId);
            }
            else if (context.Resource is DeleteUserCommand deleteUserCommand && currentContact != null)
            {
                var allowDeleteAllUsers = true;
                foreach (var userName in deleteUserCommand.UserNames)
                {
                    if (allowDeleteAllUsers)
                    {
                        var user = await _userManager.FindByNameAsync(userName);
                        allowDeleteAllUsers = await HasSameOrganizationAsync(currentContact, user.MemberId);
                    }
                }

                result = allowDeleteAllUsers;
            }
            else if (context.Resource is UpdateContactAddressesCommand updateContactAddressesCommand && currentContact != null)
            {
                result = updateContactAddressesCommand.ContactId == currentContact.Id;

                if (!result)
                {
                    result = await HasSameOrganizationAsync(currentContact, updateContactAddressesCommand.ContactId);
                }
            }
            else if (context.Resource is UpdateContactCommand updateContactCommand && currentContact != null)
            {
                result = updateContactCommand.Id == currentContact.Id;
                if (!result)
                {
                    result = await HasSameOrganizationAsync(currentContact, updateContactCommand.Id);
                }
            }
            else if (context.Resource is UpdateOrganizationCommand updateOrganizationCommand && currentContact != null)
            {
                result = currentContact.Organizations.Contains(updateOrganizationCommand.Name);
            }
            else if (context.Resource is UpdateRoleCommand updateRoleCommand)
            {
                //Can be checked only with platform permission
                result = true;
            }
            else if (context.Resource is UpdateUserCommand updateUserCommand && currentContact != null)
            {
                result = updateUserCommand.MemberId == currentContact.Id;
                if (!result)
                {
                    result = await HasSameOrganizationAsync(currentContact, updateUserCommand.Id);
                }
            }

            if (result)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }

        private static string GetUserId(AuthorizationHandlerContext context)
        {
            //TODO use ClaimTypes instead of "name"
            return context.User.FindFirstValue("name");
        }
        private async Task<bool> HasSameOrganizationAsync(Contact currentContact, string contactId)
        {
            var contact = await GetCustomerAsync(contactId) as Contact;
            return currentContact.Organizations.Intersect(contact?.Organizations ?? Array.Empty<string>()).Any();
        }

        //TODO: DRY violation in many places in this solution. Move to abstraction to from multiple boundaries
        protected virtual async Task<Member> GetCustomerAsync(string customerId)
        {
            // Try to find contact
            var result = await _memberService.GetByIdAsync(customerId);

            if (result == null)
            {
                var user = await _userManager.FindByIdAsync(customerId);

                if (user != null && user.MemberId != null)
                {
                    result = await _memberService.GetByIdAsync(user.MemberId);
                }
            }

            return result;
        }
    }
}
