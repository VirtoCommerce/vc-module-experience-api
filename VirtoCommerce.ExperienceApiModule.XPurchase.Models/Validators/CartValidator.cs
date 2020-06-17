using FluentValidation;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Validators
{
    public class CartValidator : AbstractValidator<ShoppingCart>
    {
        public CartValidator(ICartService cartService)
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Currency).NotNull();
            RuleFor(x => x.CustomerId).NotNull().NotEmpty();

            RuleSet("strict", () =>
            {
                RuleForEach(x => x.Items).SetValidator(cart => new CartLineItemValidator(cart));
                RuleForEach(x => x.Shipments).SetValidator(cart => new CartShipmentValidator(cart, cartService));
            });
        }
    }
}
