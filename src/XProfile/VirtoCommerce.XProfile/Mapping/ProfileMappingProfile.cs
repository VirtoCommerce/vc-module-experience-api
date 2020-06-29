using AutoMapper;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Mapping
{
    public class ProfileMappingProfile : Profile
    {
        public ProfileMappingProfile()
        {
            CreateMap<OrganizationUpdateInfo, Organization>().IncludeAllDerived();
        }
    }
}
