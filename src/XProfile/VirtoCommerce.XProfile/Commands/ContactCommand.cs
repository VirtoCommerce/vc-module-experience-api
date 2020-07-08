using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public abstract class ContactCommand : Contact, ICommand<ContactAggregate>
    {
        protected ContactCommand()
        {
            MemberType = nameof(Contact);
        }

        protected ContactCommand(string salutation,
            string fullName = default,
            string firstName = default,
            string middleName = default,
            string lastName = default,
            string defaultLanguage = default,
            string timeZone = default,
            IList<string> organizations = default,
            string photoUrl = default,
            //IList<ApplicationUser> securityAccounts = default(IList<ApplicationUser>),
            string name = default,
            string memberType = nameof(Contact),
            IList<Address> addresses = default,
            IList<string> phones = default,
            IList<string> emails = default,
            IList<string> groups = default
            //IList<DynamicObjectProperty> dynamicProperties = default(IList<DynamicObjectProperty>),
            //string id = default
            ) : this()
        {
            Salutation = salutation;
            FullName = fullName;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            DefaultLanguage = defaultLanguage;
            TimeZone = timeZone;
            OrganizationsIds = organizations;
            PhotoUrl = photoUrl;
            Name = name;
            MemberType = memberType;
            Addresses = addresses;
            Phones = phones;
            Emails = emails;
            Groups = groups;
        }



        public Address DefaultBillingAddress { get; set; }
        public Address DefaultShippingAddress { get; set; }

        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }
        //TODO: It needs to be rework to support only a multiple  organizations for a customer by design.
        public IList<string> OrganizationsIds { get; set; } = new List<string>();

        /// <summary>
        /// Returns true if the customer accepts marketing, returns false if the customer does not.
        /// </summary>
        public bool AcceptsMarketing { get; set; }

        /// <summary>
        /// Returns the default customer_address.
        /// </summary>
        public Address DefaultAddress { get; set; }

        //TODO
        /// <summary>
        /// All contact security accounts
        /// </summary>
        //public IEnumerable<SecurityAccount> SecurityAccounts { get; set; }

        public IList<string> PhoneNumbers { get; set; } = new List<string>();

        /// <summary>
        /// User groups such as VIP, Wholesaler etc
        /// </summary>
        public IList<string> UserGroups { get; set; } = new List<string>();
    }
}
