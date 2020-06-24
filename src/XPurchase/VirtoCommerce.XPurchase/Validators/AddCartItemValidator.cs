using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog;
using VirtoCommerce.XPurchase.Domain.CartAggregate;

namespace VirtoCommerce.XPurchase.Models.Validators
{
    public class AddCartItemValidator : AbstractValidator<NewCartItem>
    {
        public AddCartItemValidator(ShoppingCart cart)
        {
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.Product).NotNull();
            RuleSet("strict", () =>
            {
                RuleFor(x => x.Product).Must((addCartItem, product) => new ProductIsAvailableSpecification(product).IsSatisfiedBy(addCartItem.Quantity));
                RuleFor(x => x).Custom((addCartItem, context) =>
                {
                    if (addCartItem.Price != null)
                    {
                        var productSalePrice = addCartItem.Product.Price.GetTierPrice(addCartItem.Quantity).Price;
                        var newSalePrice = new Money(addCartItem.Price.Value, cart.Currency);
                        if (newSalePrice < productSalePrice)
                        {
                            context.AddFailure(new ValidationFailure(nameof(addCartItem.Price), "Unable to set less price"));
                        }
                    }
                });
            });

        }
    }
}
