using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartValidationContextFactory : ICartValidationContextFactory
    {
        private readonly ICartAvailMethodsService _availMethods;
        private readonly ICartProductService _cartProducts;

        public CartValidationContextFactory(ICartAvailMethodsService availMethods, ICartProductService cartProducs)
        {
            _availMethods = availMethods;
            _cartProducts = cartProducs;
        }

        public async Task<CartValidationContext> CreateValidationContextAsync(CartAggregate cartAggr)
        {
            var availPaymentsTask = _availMethods.GetAvailablePaymentMethodsAsync(cartAggr);
            var availShippingRatesTask = _availMethods.GetAvailableShippingRatesAsync(cartAggr);
            var cartProductsTask = _cartProducts.GetCartProductsByIdsAsync(cartAggr, cartAggr.Cart.Items.Select(x => x.ProductId).ToArray());
            await Task.WhenAll(availPaymentsTask, availShippingRatesTask, cartProductsTask);

            return new CartValidationContext
            {
                CartAggregate = cartAggr,
                AllCartProducts = cartProductsTask.Result,
                AvailPaymentMethods = availPaymentsTask.Result,
                AvailShippingRates = availShippingRatesTask.Result,
            };
        }
    }
}
