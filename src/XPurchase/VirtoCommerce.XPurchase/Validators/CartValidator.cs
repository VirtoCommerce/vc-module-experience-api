using FluentValidation;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartValidator : AbstractValidator<CartAggregate>
    {
        public CartValidator()
        {
            RuleFor(x => x.Cart.Name).NotNull().NotEmpty();
            RuleFor(x => x.Cart.Currency).NotNull();
            RuleFor(x => x.Cart.CustomerId).NotNull().NotEmpty();

            RuleSet("strict", () =>
            {
                RuleForEach(x => x.Cart.Items).SetValidator(cartAggr => new CartLineItemValidator(cartAggr));
                RuleForEach(x => x.Cart.Shipments).SetValidator(cartAggr => new CartShipmentValidator(cartAggr));
                RuleForEach(x => x.Cart.Payments).SetValidator(cartAggr => new CartPaymentValidator(cartAggr));
            });
        }
    }
}
