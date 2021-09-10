using System.Linq;
using FluentValidation;

namespace VirtoCommerce.XPurchase.Validators
{
    public class ChangeCartItemPriceValidator : AbstractValidator<PriceAdjustment>
    {
        public ChangeCartItemPriceValidator()
        {
            RuleFor(x => x.NewPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.LineItemId).NotNull().NotEmpty();
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((newPriceRequest, context) =>
                {
                    if (newPriceRequest.LineItem != null)
                    {
                        var newSalePrice = newPriceRequest.NewPrice;
                        if (newPriceRequest.LineItem.SalePrice > newSalePrice)
                        {
                            context.AddFailure(CartErrorDescriber.UnableToSetLessPrice(newPriceRequest.LineItem));
                        }
                    }
                });
            });
        }
    }
}
