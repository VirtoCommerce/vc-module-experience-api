using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates.Contact;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class ContactAggregate : Entity, IAggregateRoot
    {
        public ContactAggregate(Contact contact)
        {
            Contact = contact;
            if (string.IsNullOrEmpty(contact.FullName))
            {
                contact.FullName = string.Join(" ", contact.FirstName, contact.LastName);
            }
        }

        public Contact Contact { get; protected set; }


        public virtual ContactAggregate UpdatePersonalDetails(PersonalData personalDetails)
        {
            Contact.FirstName = personalDetails.FirstName;
            Contact.LastName = personalDetails.LastName;
            Contact.MiddleName = personalDetails.MiddleName;
            Contact.FullName = personalDetails.FullName;

            return this;
        }

        public virtual ContactAggregate UpdateContactAddresses(IList<Address> addresses)
        {
            Contact.Addresses = addresses.ToList();

            return this;
        }
    }
}
