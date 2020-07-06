using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class OrganizationAggregate : Entity, IAggregateRoot
    {
        public Organization Organization { get; protected set; }

        public void SetOrganization(Organization organization)
        {
            Organization = organization;
        }
    }
}
