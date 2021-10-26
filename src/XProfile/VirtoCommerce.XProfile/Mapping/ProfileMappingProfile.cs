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
                .ConvertUsing((command, contact, context) =>
                {
                    contact = new Contact
                    {
                        Name = command.Name,
                        PhotoUrl = command.PhotoUrl,
                        TimeZone = command.TimeZone,
                        DefaultLanguage = command.DefaultLanguage,
                        LastName = command.LastName,
                        MiddleName = command.MiddleName,
                        FirstName = command.FirstName,
                        FullName = command.FullName,
                        Salutation = command.Salutation,
                        Addresses = command.Addresses,
                        Phones = command.Phones,
                        Emails = command.Emails,
                        Groups = command.Groups,
                        Organizations = command.Organizations,
                    };
                    return contact;
                });
            CreateMap<UpdateContactCommand, Contact>().ForMember(x => x.DynamicProperties, opt => opt.Ignore());
        }
    }
}
