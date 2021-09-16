using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.XPurchase
{
    public class GiftItem : GiftReward
    {
        // CatalogId to pass to lineItem
        public string CatalogId { get; set; }
        // Sku to pass to lineItem
        public string Sku { get; set; }
        public string LineItemId { get; set; }


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PromotionId;
            yield return Coupon;
            yield return Name;
            yield return CategoryId;
            yield return ProductId;
            yield return Quantity;
            yield return MeasureUnit;
            yield return ImageUrl;
        }
    }
}
