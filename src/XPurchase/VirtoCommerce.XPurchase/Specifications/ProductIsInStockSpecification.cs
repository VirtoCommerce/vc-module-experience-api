namespace VirtoCommerce.XPurchase.Specifications
{
    public class ProductIsInStockSpecification 
    {
        public virtual bool IsSatisfiedBy(CartProduct product)
        {
            var result = true;
            if (product.Product.TrackInventory.GetValueOrDefault(false) && product.Inventory != null)
            {
                result = product.Inventory.AllowPreorder || product.Inventory.AllowBackorder || product.AvailableQuantity > 0;
            }
            return result;
        }

    }
}
