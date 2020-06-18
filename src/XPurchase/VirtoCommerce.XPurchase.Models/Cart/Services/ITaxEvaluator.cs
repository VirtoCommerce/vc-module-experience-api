using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Models.Tax;

namespace VirtoCommerce.XPurchase.Models.Cart.Services
{
    public interface ITaxEvaluator
    {
        Task EvaluateTaxesAsync(TaxEvaluationContext context, IEnumerable<ITaxable> owners);
    }
}
