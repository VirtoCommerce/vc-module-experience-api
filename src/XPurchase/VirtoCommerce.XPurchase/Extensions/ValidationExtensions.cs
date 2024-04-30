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
    }
}
