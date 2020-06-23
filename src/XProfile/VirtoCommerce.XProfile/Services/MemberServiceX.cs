using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Services
{
    public class MemberServiceX : IMemberServiceX
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;

        public MemberServiceX(IAuthorizationService authorizationService, IMemberService memberService, IMemberSearchService memberSearchService)
        {
            _authorizationService = authorizationService;
            _memberService = memberService;
            _memberSearchService = memberSearchService;
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
            throw new NotImplementedException();
        }

        public Task<Organization> GetOrganizationByIdAsync(string organizationId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateContactAddressesAsync(string memberId, IList<Address> addresses)
        {
            var member = await _memberService.GetByIdAsync(memberId);
            if (member != null)
            {
                //if (!(await AuthorizeAsync(member, ModuleConstants.Security.Permissions.Update)).Succeeded)
                //{
                //    return Unauthorized();
                //}
                member.Addresses = addresses.ToList();
                await _memberService.SaveChangesAsync(new[] { member });
            }
        }

        public Task UpdateContactAsync(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrganizationAsync(Organization organization)
        {
            throw new NotImplementedException();
        }
    }
}
