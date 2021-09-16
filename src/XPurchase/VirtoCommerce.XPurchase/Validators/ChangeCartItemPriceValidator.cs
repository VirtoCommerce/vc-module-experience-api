using System.Linq;
using FluentValidation;

namespace VirtoCommerce.XPurchase.Validators
{
    public class ChangeCartItemPriceValidator : AbstractValidator<PriceAdjustment>
    {
        public ChangeCartItemPriceValidator(CartAggregate cartAggr)
        {
            RuleFor(x => x.NewPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.LineItemId).NotNull().NotEmpty();
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((newPriceRequest, context) =>
                {
                    var lineItem = cartAggr.Cart.Items.FirstOrDefault(x => x.Id == newPriceRequest.LineItemId);
                    if (lineItem != null)
                    {
                        if (lineItem.IsGift)
                        {
                            context.AddFailure(CartErrorDescriber.LineItemIsReadOnly(lineItem));
                        }

                        var newSalePrice = newPriceRequest.NewPrice;
                        if (lineItem.SalePrice > newSalePrice)
                        {
                            context.AddFailure(CartErrorDescriber.UnableToSetLessPrice(lineItem));
                        }
                    }
                });
            });
        }
    }
}
