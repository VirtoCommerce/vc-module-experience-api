using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public partial class OrganizationUpdateInfo : Entity
    {
        public string Name { get; set; }
        public IList<Address> Addresses { get; set; }
    }
}
