using System.Collections.Generic;
using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Customer
{
    public class SecurityAccount : Entity
    {
        public string UserName { get; set; }
        public bool IsLockedOut { get; set; }
        public IList<string> Roles { get; set; }
    }
}
