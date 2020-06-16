using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.ValidationErrors;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart
{
    public interface IValidatable
    {
        bool IsValid { get; set; }

        IList<ValidationError> ValidationErrors { get; }
    }
}
