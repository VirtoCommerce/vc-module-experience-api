using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart
{
    public class CartShipmentItem : CloneableEntity
    {
        public LineItem LineItem { get; set; }

        public int Quantity { get; set; }

        public override object Clone()
        {
            var result = base.Clone() as CartShipmentItem;

            result.LineItem = LineItem?.Clone() as LineItem;
            return result;
        }
    }
}
