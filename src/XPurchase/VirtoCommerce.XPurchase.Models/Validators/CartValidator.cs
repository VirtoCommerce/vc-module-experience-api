using System.Collections.Generic;
using FluentValidation;
using VirtoCommerce.XPurchase.Models.Cart;

namespace VirtoCommerce.XPurchase.Models.Validators
{
    public class CartValidator : AbstractValidator<ShoppingCart>
    {
        public CartValidator(IEnumerable<ShippingMethod> availableShippingMethods)
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Currency).NotNull();
            RuleFor(x => x.CustomerId).NotNull().NotEmpty();

            RuleSet("strict", () =>
            {
                RuleForEach(x => x.Items).SetValidator(cart => new CartLineItemValidator(cart));
                RuleForEach(x => x.Shipments).SetValidator(cart => new CartShipmentValidator(availableShippingMethods));
            });
        }
    }
}
