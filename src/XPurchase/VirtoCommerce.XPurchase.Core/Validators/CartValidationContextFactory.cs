using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Core.Validators
{
    public class CartValidationContextFactory : ICartValidationContextFactory
    {
        private readonly ICartAvailMethodsService _availMethods;
        private readonly ICartProductService _cartProducts;

        public CartValidationContextFactory(ICartAvailMethodsService availMethods, ICartProductService cartProducts)
        {
            _availMethods = availMethods;
            _cartProducts = cartProducts;
        }

        public async Task<CartValidationContext> CreateValidationContextAsync(CartAggregate cartAggregate)
        {
            var availPaymentsTask = _availMethods.GetAvailablePaymentMethodsAsync(cartAggregate);
            var availShippingRatesTask = _availMethods.GetAvailableShippingRatesAsync(cartAggregate);
            var cartProductsTask = _cartProducts.GetCartProductsByIdsAsync(cartAggregate, cartAggregate.Cart.Items.Select(x => x.ProductId).ToArray());
            await Task.WhenAll(availPaymentsTask, availShippingRatesTask, cartProductsTask);

            return new CartValidationContext
            {
                CartAggregate = cartAggregate,
                AllCartProducts = cartProductsTask.Result,
                AvailPaymentMethods = availPaymentsTask.Result,
                AvailShippingRates = availShippingRatesTask.Result,
            };
        }
    }
}
