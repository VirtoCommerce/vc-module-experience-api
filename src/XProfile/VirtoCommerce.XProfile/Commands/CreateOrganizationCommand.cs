using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateOrganizationCommand : ICommand<OrganizationAggregate>
    {
        public CreateOrganizationCommand(string name, IList<Address> addresses)
        {
            Name = name;
            Addresses = addresses;
        }

        public string Name { get; set; }
        public IList<Address> Addresses { get; set; } = new List<Address>();
        public string UserId { get; set; }
    }
}
