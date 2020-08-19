using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Mapping
{
    public class ProfileMappingProfile : AutoMapper.Profile
    {
        public ProfileMappingProfile()
        {
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
            CreateMap<UpdateOrganizationCommand, Organization>();
        }
    }
}
