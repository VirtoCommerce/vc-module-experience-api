namespace VirtoCommerce.XPurchase
{
    public class ProductIsAvailableSpecification
    {
        public virtual bool IsSatisfiedBy(CartProduct product, long requestedQuantity)
        {
            var result = new ProductIsBuyableSpecification().IsSatisfiedBy(product) && product.Inventory != null;

            if (result && product.Product.TrackInventory.GetValueOrDefault(false))
            {
                result = product.Inventory.AllowPreorder ||
                              product.Inventory.AllowBackorder ||
                              product.AvailableQuantity >= requestedQuantity;
            }

            return result;
        }

    }
}
