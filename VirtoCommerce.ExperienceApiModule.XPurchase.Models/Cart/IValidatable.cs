using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart
{
    public interface IValidatable
    {
        bool IsValid { get; set; }

        IList<ValidationError> ValidationErrors { get; }
    }
}
