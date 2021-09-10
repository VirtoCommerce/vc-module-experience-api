using System.Linq;
using FluentValidation;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartLineItemValidator : AbstractValidator<LineItemValidationContext>
    {
        public CartLineItemValidator()
        {
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((lineItemContext, context) =>
                {
                    var lineItem = lineItemContext.LineItem;
                    var allCartProducts = lineItemContext.AllCartProducts;

                    var cartProduct = allCartProducts.FirstOrDefault(x => x.Id.EqualsInvariant(lineItem.ProductId));
                    if (cartProduct == null || !AbstractTypeFactory<ProductIsBuyableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct))
                    {
                        context.AddFailure(CartErrorDescriber.ProductUnavailableError(lineItem));
                    }
                    else
                    {
                        var isProductAvailable = AbstractTypeFactory<ProductIsAvailableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct, lineItem.Quantity);
                        if (!isProductAvailable)
                        {
                            context.AddFailure(CartErrorDescriber.ProductQtyChangedError(lineItem, cartProduct.AvailableQuantity));
                        }

                        var tierPrice = cartProduct.Price.GetTierPrice(lineItem.Quantity);
                        if (tierPrice.Price.Amount > lineItem.SalePrice)
                        {
                            context.AddFailure(CartErrorDescriber.ProductPriceChangedError(lineItem, lineItem.SalePrice, lineItem.SalePriceWithTax, tierPrice.Price.Amount, tierPrice.PriceWithTax.Amount));
                        }
                    }
                });
            });
        }
    }
}
