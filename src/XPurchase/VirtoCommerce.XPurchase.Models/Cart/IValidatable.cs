using System.Collections.Generic;
using VirtoCommerce.XPurchase.Models.Cart.ValidationErrors;

namespace VirtoCommerce.XPurchase.Models.Cart
{
    public interface IValidatable
    {
        bool IsValid { get; }

        IList<ValidationError> ValidationErrors { get; }
    }
}
