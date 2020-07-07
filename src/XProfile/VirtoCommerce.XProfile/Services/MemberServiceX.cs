using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Services
{
    public class MemberServiceX : IMemberServiceX
    {
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;
        private readonly IServiceProvider _services;
        private readonly IMapper _mapper;

        public MemberServiceX(IMemberService memberService, IMemberSearchService memberSearchService, IServiceProvider services, IMapper mapper)
        {
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _services = services;
            _mapper = mapper;
        }

        public Task<Contact> CreateContactAsync(Contact contact)
        {
            throw new NotImplementedException();
        }

        public async Task<Organization> CreateOrganizationAsync(CreateOrganizationCommand organizationRequest)
        {
            throw new NotImplementedException();
        }

        public Task DeleteContactAsync(string contactId)
        {
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

        //public async Task<Profile> UpdateContactAsync(UserUpdateInfo userUpdateInfo)
        //{
        //    if (!string.IsNullOrEmpty(userUpdateInfo.Id))
        //    {
        //        if (!user.MemberId.IsNullOrEmpty())
        //        {
        //            var member = await _memberService.GetByIdAsync(user.MemberId);
        //            if (member != null)
        //            {
        //                _mapper.Map(userUpdateInfo, member);
        //                member.Id = user.MemberId;

        //                await _memberService.SaveChangesAsync(new[] { member });
        //            }
        //        }
        //    }

        //    return default;
        //}

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
    }
}
