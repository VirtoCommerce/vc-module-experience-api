using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class ContactAggregate : Entity, IAggregateRoot
    {
        public ContactAggregate(Contact contact)
        {
            Contact = contact;
        }

        public Contact Contact { get; protected set; }


        public virtual ContactAggregate UpdateContactAddresses(IList<Address> addresses)
        {
            Contact.Addresses = addresses.ToList();

            return this;
        }
    }
}
