using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Services
{
    public interface IMemberServiceX
    {
        Task<Contact> CreateContactAsync(Contact contact);
        Task<Contact> UpdateContactAddressesAsync(string contactId, IList<Address> addresses);
        Task DeleteContactAsync(string contactId);
        Task<MemberSearchResult> SearchOrganizationContactsAsync(MembersSearchCriteria criteria);

        Task<Organization> GetOrganizationByIdAsync(string organizationId);
        Task<Organization> CreateOrganizationAsync(CreateOrganizationCommand organization);
        Task<Organization> UpdateOrganizationAsync(OrganizationUpdateInfo organizationUpdateInfo);
        Task<Organization> UpdateOrganizationAsync(Organization org);


        //Task<Vendor[]> GetVendorsByIdsAsync(Store store, Language language, params string[] vendorIds);
        //Vendor[] GetVendorsByIds(Store store, Language language, params string[] vendorIds);
        //IPagedList<Vendor> SearchVendors(Store store, Language language, string keyword, int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos);
    }
}
