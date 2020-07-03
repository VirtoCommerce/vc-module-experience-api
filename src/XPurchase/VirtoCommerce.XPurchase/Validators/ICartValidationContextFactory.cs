using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Validators
{
    public interface ICartValidationContextFactory
    {
        Task<CartValidationContext> CreateValidationContextAsync(CartAggregate cartAggr);
    }
}
