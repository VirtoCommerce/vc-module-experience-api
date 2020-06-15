using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Security;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Stores;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Models
{
    public class ShoppingCartContext
    {
        public Store CurrentStore { get; set; }
        public string CartName { get; set; }
        public User User { get; set; }
        public Language Language { get; set; }
        public Currency Currency { get; set; }
        public string Type { get; set; }
    }
}
