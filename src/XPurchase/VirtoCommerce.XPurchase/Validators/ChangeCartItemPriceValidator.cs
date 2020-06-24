using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Domain.CartAggregate;

namespace VirtoCommerce.XPurchase.Models.Validators
{
    public class ChangeCartItemPriceValidator : AbstractValidator<PriceAdjustment>
    {
        public ChangeCartItemPriceValidator(ShoppingCart cart)
        {
            RuleFor(x => x.NewPrice.Amount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.LineItemId).NotNull().NotEmpty();
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((newPriceRequest, context) =>
                {
                    var lineItem = cart.Items.FirstOrDefault(x => x.Id == newPriceRequest.LineItemId);
                    if (lineItem != null)
                    {
                        var newSalePrice = newPriceRequest.NewPrice.Amount;
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
