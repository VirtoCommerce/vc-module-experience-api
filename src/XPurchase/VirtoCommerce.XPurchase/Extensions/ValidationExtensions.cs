namespace VirtoCommerce.XPurchase.Extensions
{
    public static class ValidationExtensions
    {
        public static bool IsOutsideMinMaxQuantity(int quantity, int minQuantity, int maxQuantity)
        {
            return IsBelowMinQuantity(quantity, minQuantity) || IsAboveMaxQuantity(quantity, maxQuantity);
        }

        public static bool IsBelowMinQuantity(int quantity, int? minQuantity)
        {
            return minQuantity.HasValue && minQuantity.Value > 0 && quantity < minQuantity.Value;
        }

        public static bool IsAboveMaxQuantity(int quantity, int? maxQuantity)
        {
            return maxQuantity.HasValue && maxQuantity.Value > 0 && quantity > maxQuantity.Value;
        }

        public static bool IsInventoryTrackingEnabled(this CartProduct product)
        {
            var result = product.Product.TrackInventory.GetValueOrDefault(false);

            if (product.Product.TrackInventory.GetValueOrDefault(false) && product.Inventory != null)
            {
                // no inventory tracking if preorders are allowed
                result = !product.Inventory.AllowPreorder && !product.Inventory.AllowBackorder;
            }

            return result;
        }

        public static int? GetMinQuantity(this CartProduct cartProduct)
        {
            var minQuantity = cartProduct.Product.MinQuantity;

            if (minQuantity == 0)
            {
                minQuantity = null;
            }

            return minQuantity;
        }

        public static int? GetMaxQuantity(this CartProduct cartProduct)
        {
            var maxQuantity = cartProduct.Product.MaxQuantity;

            if (maxQuantity == 0)
            {
                maxQuantity = null;
            }

            if (cartProduct.IsInventoryTrackingEnabled()
                && maxQuantity.HasValue
                && maxQuantity.Value > cartProduct.AvailableQuantity)
            {
                maxQuantity = (int)cartProduct.AvailableQuantity;
            }

            return maxQuantity;
        }
    }
}
