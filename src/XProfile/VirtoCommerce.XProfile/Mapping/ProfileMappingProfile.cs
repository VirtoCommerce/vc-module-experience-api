using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
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
            CreateMap<CreateOrganizationCommand, OrganizationAggregate>()
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src));
            CreateMap<UpdateOrganizationCommand, OrganizationAggregate>()
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src));
            CreateMap<CreateContactCommand, ContactAggregate>()
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src));
            CreateMap<UpdateContactCommand, ContactAggregate>()
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src));
            CreateMap<UpdateContactAddressesCommand, ContactAggregate>()
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src));
        }
    }
}
