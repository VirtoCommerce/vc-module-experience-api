using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.DynamicProperties;

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

        public IList<string> PhoneNumbers { get; set; } = new List<string>();
        /// <summary>
        /// Returns the email address of the customer.
        /// </summary>
        public IList<string> Emails { get; set; } = new List<string>();
        public IList<string> Phones { get; set; } = new List<string>();
        public IList<string> Groups { get; set; } = new List<string>();
        /// <summary>
        /// User groups such as VIP, Wholesaler etc
        /// </summary>
        public IList<string> UserGroups { get; set; } = new List<string>();

        public IList<Address> Addresses { get; set; } = new List<Address>();
        public IList<DynamicProperty> DynamicProperties { get; set; } = new List<DynamicProperty>(Enumerable.Empty<DynamicProperty>());
        public string UserId { get; set; }
    }
}
