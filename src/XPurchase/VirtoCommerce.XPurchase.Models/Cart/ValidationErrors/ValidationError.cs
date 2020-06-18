using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Cart.ValidationErrors
{
    public abstract class ValidationError : CloneableValueObject
    {
        protected ValidationError() => ErrorCode = GetType().Name;

        public string ErrorCode { get; private set; }
    }
}
