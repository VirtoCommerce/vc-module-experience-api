using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateOrganizationCommand : ICommand<OrganizationAggregate>
    {
        public UpdateOrganizationCommand()
        {
            MemberType = nameof(Organization);
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string MemberType { get; set; }

        public IList<string> PhoneNumbers { get; set; }
        /// <summary>
        /// Returns the email address of the customer.
        /// </summary>
        public IList<string> Emails { get; set; }
        public IList<string> Phones { get; set; }
        public IList<string> Groups { get; set; }
        /// <summary>
        /// User groups such as VIP, Wholesaler etc
        /// </summary>
        public IList<string> UserGroups { get; set; }

        public IList<Address> Addresses { get; set; }
        public string UserId { get; set; }

        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
