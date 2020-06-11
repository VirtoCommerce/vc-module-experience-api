using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services
{
    public interface ITaxEvaluator
    {
        Task EvaluateTaxesAsync(TaxEvaluationContext context, IEnumerable<ITaxable> owners);
    }
}
