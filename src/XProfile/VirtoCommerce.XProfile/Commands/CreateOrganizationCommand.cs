using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateOrganizationCommand : ICommand<OrganizationAggregate>
    {
        public string Name { get; set; }
        public IList<Address> Addresses { get; set; }

        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
