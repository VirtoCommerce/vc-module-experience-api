using System.Collections.Generic;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.XPurchase
{
    public class GiftItem : GiftReward
    {
        // CatalogId to pass to lineItem
        public string CatalogId { get; set; }
        // Sku to pass to lineItem
        public string Sku { get; set; }

        // The Id of Cart LineItem, if the gift was added to the cart.
        // Can be same as Id, it means is was added to the cart but it was not saved yet.
        public string LineItemId { get; set; }

        public bool HasLineItem { get; set; }
        public bool LineItemSelectedForCheckout { get; set; }

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

    public class GiftLineItem : LineItem
    {
        public string GiftItemId { get; set; }
    }
}
