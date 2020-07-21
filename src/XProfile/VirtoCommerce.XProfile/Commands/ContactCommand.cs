using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    //TODO: We mustn't use such general  commands that update entire contact on the xApi level. Need to use commands more close to real business scenarios instead.
    //remove in the future
    public abstract class ContactCommand : Contact, ICommand<ContactAggregate>
    {
        protected ContactCommand()
        {
            MemberType = nameof(Contact);
        }

        public Address DefaultBillingAddress { get; set; }
        public Address DefaultShippingAddress { get; set; }

        /// <summary>
        /// Returns true if the customer accepts marketing, returns false if the customer does not.
        /// </summary>
        public bool AcceptsMarketing { get; set; }

        /// <summary>
        /// Returns the default customer_address.
        /// </summary>
        public Address DefaultAddress { get; set; }


        public IList<string> PhoneNumbers
        {
            get
            {
                return Phones;
            }
            set
            {
                Phones = value;
            }
        }


        /// <summary>
        /// User groups such as VIP, Wholesaler etc
        /// </summary>
        public IList<string> UserGroups
        {
            get
            {
                return Groups;
            }
            set
            {
                Groups = value;
            }
        }
    }
}
