using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Mapping
{
    public class ProfileMappingProfile : AutoMapper.Profile
    {
        public ProfileMappingProfile()
        {
            CreateMap<Contact, Customer>();
            CreateMap<CustomerModule.Core.Model.Address, TaxModule.Core.Model.Address>();
            CreateMap<CreateOrganizationCommand, Organization>()
                .ConvertUsing((command, org, context) =>
                {
                    org = new Organization
                    {
                        Name = command.Name,
                        Addresses = command.Addresses
                    };
                    return org;
                });
            CreateMap<UpdateOrganizationCommand, Organization>().ForMember(x => x.DynamicProperties, opt => opt.Ignore());

            CreateMap<CreateContactCommand, Contact>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.DynamicProperties, opt => opt.Ignore());
            CreateMap<UpdateContactCommand, Contact>().ForMember(x => x.DynamicProperties, opt => opt.Ignore());
        }
    }
}
