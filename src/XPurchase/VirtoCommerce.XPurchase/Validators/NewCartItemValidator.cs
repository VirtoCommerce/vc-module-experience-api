using FluentValidation;

namespace VirtoCommerce.XPurchase.Validators
{
    public class NewCartItemValidator : AbstractValidator<NewCartItem>
    {
        public NewCartItemValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.ProductId).NotNull();
            RuleFor(x => x.CartProduct).NotNull();
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((newCartItem, context) =>
                {
                    if (!new ProductIsAvailableSpecification().IsSatisfiedBy(newCartItem.CartProduct, newCartItem.Quantity))
                    {
                        context.AddFailure(CartErrorDescriber.ProductUnavailableError(newCartItem.CartProduct));
                    }
                    if (newCartItem.Price != null)
                    {
                        var productSalePrice = newCartItem.CartProduct.Price.GetTierPrice(newCartItem.Quantity).Price;

                        if (newCartItem.Price < productSalePrice.Amount)
                        {
                            context.AddFailure(CartErrorDescriber.UnableToSetLessPrice(newCartItem.CartProduct));
                        }
                    }
                });
            });
        }
    }
}
