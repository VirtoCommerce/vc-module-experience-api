using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Authorization
{
    public class ProfileAuthorizationRequirement : IAuthorizationRequirement
    {
        public ProfileAuthorizationRequirement()
        {
        }
    }

    public class ProfileAuthorizationHandler : AuthorizationHandler<ProfileAuthorizationRequirement>
    {
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;


        public ProfileAuthorizationHandler(IMemberService memberService, Func<UserManager<ApplicationUser>> userManager)
        {
            _memberService = memberService;
            _userManager = userManager();
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileAuthorizationRequirement requirement)
        {
            var result = context.User.IsInRole(PlatformConstants.Security.SystemRoles.Administrator);

            // Administrators can do anything except creating any users
            if (result && !(context.Resource is CreateUserCommand))
            {
                context.Succeed(requirement);
                return;
            }

            var currentUserId = GetUserId(context);
            var currentContact = await GetCustomerAsync(currentUserId) as Contact;

            if (context.Resource is ContactAggregate contactAggregate && currentContact != null)
            {
                result = currentContact.Id == contactAggregate.Contact.Id;
                if (!result)
                {
                    result = await HasSameOrganizationAsync(currentContact, contactAggregate.Contact.Id);
                }
            }
            else if (context.Resource is ApplicationUser applicationUser)
            {
                result = currentUserId == applicationUser.Id;
                if (!result)
                {
                    result = await HasSameOrganizationAsync(currentContact, applicationUser.Id);
                }
            }
            else if (context.Resource is OrganizationAggregate organizationAggregate && currentContact != null)
            {
                result = currentContact.Organizations.Contains(organizationAggregate.Organization.Id);
            }

            else if (context.Resource is Role role)
            {
                //Can be checked only with platform permission
                result = true;
            }
            else if (context.Resource is CreateContactCommand createContactCommand)
            {
                //Anonymous user can create contact
                result = true;
            }
            else if (context.Resource is CreateOrganizationCommand createOrganizationCommand)
            {
                //New user can create organization on b2b-theme
                result = true;
            }
            else if (context.Resource is CreateUserCommand createUserCommand)
            {
                //Anonymous user can create customer users only
                result = !createUserCommand.ApplicationUser.IsAdministrator && createUserCommand.ApplicationUser.UserType.EqualsInvariant("Customer");
            }
            else if (context.Resource is SendVerifyEmailCommand)
            {
                //Anonymous user request verification email
                result = true;
            }
            else if (context.Resource is DeleteContactCommand deleteContactCommand && currentContact != null)
            {
                result = await HasSameOrganizationAsync(currentContact, deleteContactCommand.ContactId);
            }
            else if (context.Resource is DeleteUserCommand deleteUserCommand && currentContact != null)
            {
                var allowDelete = true;
                foreach (var userName in deleteUserCommand.UserNames)
                {
                    if (allowDelete)
                    {
                        var user = await _userManager.FindByNameAsync(userName);
                        allowDelete = await HasSameOrganizationAsync(currentContact, user.MemberId);
                    }
                }

                result = allowDelete;
            }
            else if (context.Resource is UpdateMemberAddressesCommand updateMemberAddressesCommand)
            {
                result = updateMemberAddressesCommand.MemberId == currentUserId;
                if (!result && currentContact != null)
                {
                    var memberId = updateMemberAddressesCommand.MemberId;
                    var member = await _memberService.GetByIdAsync(memberId);

                    if (member.MemberType.EqualsInvariant("Organization") && currentContact.Organizations.Any(x => x.EqualsInvariant(member.Id)))
                    {
                        result = true;
                    }
                    else
                    {
                        result = await HasSameOrganizationAsync(currentContact, memberId);
                    }
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
                result = currentContact.Organizations.Contains(updateOrganizationCommand.Id);
            }
            else if (context.Resource is UpdateMemberDynamicPropertiesCommand)
            {
                //Can be checked only with platform permission
                result = true;
            }
            else if (context.Resource is UpdateRoleCommand updateRoleCommand)
            {
                //Can be checked only with platform permission
                result = true;
            }
            else if (context.Resource is UpdateUserCommand updateUserCommand && currentContact != null)
            {
                result = updateUserCommand.ApplicationUser.Id == currentContact.Id;
                if (!result)
                {
                    result = await HasSameOrganizationAsync(currentContact, updateUserCommand.ApplicationUser.Id);
                }
            }
            else if (context.Resource is UpdatePersonalDataCommand updatePersonalDataCommand)
            {
                updatePersonalDataCommand.UserId = currentUserId;
                result = true;
            }
            else if (context.Resource is InviteUserCommand inviteUserCommand && currentContact != null)
            {
                var currentUser = await _userManager.FindByIdAsync(currentUserId);
                result = currentContact.Organizations.Contains(inviteUserCommand.OrganizationId) && currentUser.StoreId.EqualsInvariant(inviteUserCommand.StoreId);
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
            //PT-5375 use ClaimTypes instead of "name"
            return context.User.FindFirstValue("name");
        }

        private async Task<bool> HasSameOrganizationAsync(Contact currentContact, string contactId)
        {
            if (currentContact is null)
                return false;

            var contact = await GetCustomerAsync(contactId) as Contact;
            return currentContact.Organizations.Intersect(contact?.Organizations ?? Array.Empty<string>()).Any();
        }

        //TODO: DRY violation in many places in this solution. Move to abstraction to from multiple boundaries
        protected virtual async Task<Member> GetCustomerAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return null;
            }

            var result = await _memberService.GetByIdAsync(customerId);

            if (result == null)
            {
                var user = await _userManager.FindByIdAsync(customerId);

                if (user?.MemberId != null)
                {
                    result = await _memberService.GetByIdAsync(user.MemberId);
                }
            }

            return result;
        }
    }
}
