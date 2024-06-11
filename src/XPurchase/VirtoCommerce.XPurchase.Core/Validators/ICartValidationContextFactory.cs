using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Core.Validators
{
    public interface ICartValidationContextFactory
    {
        Task<CartValidationContext> CreateValidationContextAsync(CartAggregate cartAggregate);
    }
}
