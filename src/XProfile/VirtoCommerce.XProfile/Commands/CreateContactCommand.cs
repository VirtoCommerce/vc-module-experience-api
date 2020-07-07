using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateContactCommand : ContactCommand
    {
        public CreateContactCommand()
        {

        }

        public CreateContactCommand(string salutation,
            string fullName = default(string),
            string firstName = default(string),
            string middleName = default(string),
            string lastName = default(string),
            string defaultLanguage = default(string),
            string timeZone = default(string),
            IList<string> organizations = default(IList<string>),
            string photoUrl = default(string),
            //IList<ApplicationUser> securityAccounts = default(IList<ApplicationUser>),
            string name = default(string),
            string memberType = default(string),
            IList<Address> addresses = default(IList<Address>),
            IList<string> phones = default(IList<string>),
            IList<string> emails = default(IList<string>),
            IList<string> groups = default(IList<string>)
            //IList<DynamicObjectProperty> dynamicProperties = default(IList<DynamicObjectProperty>),
            //string id = default(string)
            )
            : base(
                salutation,
                fullName,
                firstName,
                middleName,
                lastName,
                defaultLanguage,
                timeZone,
                organizations,
                photoUrl,
                //IList<ApplicationUser> securityAccounts = default(IList<ApplicationUser>),
                name,
                memberType,
                addresses,
                phones,
                emails,
                groups
                //IList<DynamicObjectProperty> dynamicProperties = default(IList<DynamicObjectProperty>),
                //string id = default(string)
                )
        {

        }
    }
}
