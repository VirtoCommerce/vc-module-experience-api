namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.ValidationErrors
{
    public class QuantityError : ValidationError
    {
        public QuantityError(long availableQuantity)
        {
            AvailableQuantity = availableQuantity;
        }

        public long AvailableQuantity { get; private set; }
    }
}
