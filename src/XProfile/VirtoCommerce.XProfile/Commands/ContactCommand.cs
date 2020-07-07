using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public abstract class ContactCommand : ICommand<ContactAggregate>
    {
        protected ContactCommand()
        {
        }

        protected ContactCommand(string salutation,
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

        public string FullName { get; set; }
        /// <summary>
        /// Returns the first name of the customer.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Returns the last name of the customer.
        /// </summary>
        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Salutation { get; set; }

        public string PhotoUrl { get; set; }

        public string TimeZone { get; set; }
        public string DefaultLanguage { get; set; }

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

        public string Name { get; set; }
        public string MemberType { get; set; }

        public IList<string> PhoneNumbers { get; set; } = new List<string>();
        public IList<string> Emails { get; set; } = new List<string>();

        
        public IList<Address> Addresses { get; set; } = new List<Address>();
        public IList<string> Phones { get; set; } = new List<string>();
        public IList<string> Groups { get; set; } = new List<string>();

        /// <summary>
        /// User groups such as VIP, Wholesaler etc
        /// </summary>
        public IList<string> UserGroups { get; set; } = new List<string>();
        public IList<DynamicProperty> DynamicProperties { get; set; } = new List<DynamicProperty>(Enumerable.Empty<DynamicProperty>());
    }
}
