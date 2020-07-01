using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Mapping
{
    public class ProfileMappingProfile : AutoMapper.Profile
    {
        public ProfileMappingProfile()
        {
            CreateMap<UserUpdateInfo, Contact>().IncludeAllDerived();
            CreateMap<UserUpdateInfo, ApplicationUser>();
            CreateMap<OrganizationUpdateInfo, Member>().IncludeAllDerived();
        }
    }
}
