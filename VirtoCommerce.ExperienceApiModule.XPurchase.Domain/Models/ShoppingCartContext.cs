using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Stores;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Models
{
    public class ShoppingCartContext
    {
        public string StoreId { get; set; }
        public Store Store { get; private set; } = new Store();
        public void SetStore(Store store) => Store = store;

        public string CartName { get; set; }
        public string UserId { get; set; }
        public Language Language { get; set; }
        public Currency Currency { get; set; }
        public string Type { get; set; }
    }
}
