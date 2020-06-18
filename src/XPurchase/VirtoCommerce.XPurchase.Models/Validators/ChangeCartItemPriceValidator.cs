using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.XPurchase.Models.Cart;
using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Validators
{
    public class ChangeCartItemPriceValidator : AbstractValidator<ChangeCartItemPrice>
    {
        public ChangeCartItemPriceValidator(ShoppingCart cart)
        {
            RuleFor(x => x.NewPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.LineItemId).NotNull().NotEmpty();
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((newPriceRequest, context) =>
                {
                    var lineItem = cart.Items.FirstOrDefault(x => x.Id == newPriceRequest.LineItemId);
                    if (lineItem != null)
                    {
                        var newSalePrice = new Money(newPriceRequest.NewPrice, cart.Currency);
                        if (lineItem.SalePrice > newSalePrice)
                        {
                            context.AddFailure(new ValidationFailure(nameof(lineItem.SalePrice), "Unable to set less price"));
                        }

                    }
                });
            });

        }
    }
}
