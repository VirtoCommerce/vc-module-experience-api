using System.Collections.Generic;
using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Marketing
{
    public interface IDiscountable
    {
        Currency Currency { get; }

        IList<Discount> Discounts { get; }

        void ApplyRewards(IEnumerable<PromotionReward> rewards);
    }
}
