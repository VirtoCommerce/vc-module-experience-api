using FluentValidation;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartValidator : AbstractValidator<CartAggregate>
    {
        public CartValidator(CartValidationContext validationContext)
        {
            RuleFor(x => x.Cart.Name).NotNull().NotEmpty();
            RuleFor(x => x.Cart.Currency).NotNull();
            RuleFor(x => x.Cart.CustomerId).NotNull().NotEmpty();

            RuleSet("strict", () =>
            {
                RuleForEach(x => x.Cart.Items).SetValidator(cartAggr => new CartLineItemValidator(validationContext.AllCartProducts ?? cartAggr.CartProducts.Values));
                RuleForEach(x => x.Cart.Shipments).SetValidator(cartAggr => new CartShipmentValidator(validationContext.AvailShippingRates));
                RuleForEach(x => x.Cart.Payments).SetValidator(cartAggr => new CartPaymentValidator(validationContext.AvailPaymentMethods));
            });
        }
    }
}
