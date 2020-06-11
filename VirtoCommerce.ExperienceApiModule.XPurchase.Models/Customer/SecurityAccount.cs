using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Customer
{
    public class SecurityAccount : Entity
    {
        public string UserName { get; set; }
        public bool IsLockedOut { get; set; }
        public IList<string> Roles { get; set; }
    }
}
