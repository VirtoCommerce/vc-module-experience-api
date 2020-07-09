using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Mapping
{
    public class ProfileMappingProfile : AutoMapper.Profile
    {
        public ProfileMappingProfile()
        {
            CreateMap<CreateOrganizationCommand, OrganizationAggregate>()
                .ConvertUsing((command, aggregate, context) =>
                {
                    aggregate = new OrganizationAggregate(AbstractTypeFactory<Organization>.TryCreateInstance());
                    aggregate.Organization.Name = command.Name;
                    aggregate.Organization.Addresses = command.Addresses;

                    return aggregate;
                });
            CreateMap<UpdateOrganizationCommand, Organization>();
            CreateMap<CreateContactCommand, Contact>();
            CreateMap<UpdateContactCommand, Contact>();
        }
    }
}
