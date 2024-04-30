namespace VirtoCommerce.XPurchase
{
    public class ProductMinQunatityAvailableSpecification
    {
        public virtual bool IsSatisfiedBy(CartProduct product, int? minQuantity)
        {
            var result = true;

            if (minQuantity.HasValue && minQuantity.Value > 0 && product.Product.TrackInventory.GetValueOrDefault(false))
            {
                result = product.AvailableQuantity >= minQuantity.Value;
            }

            return result;
        }
    }
}
