using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartLineItemValidator : AbstractValidator<LineItem>
    {
        public CartLineItemValidator(CartAggregate cartAggr)
        {
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((lineItem, context) =>
                {
                    var cartProduct = cartAggr.CartProductsDict[lineItem.ProductId];
                    if (cartProduct == null || !cartProduct.Product.IsActive.GetValueOrDefault(false) || !cartProduct.Product.IsBuyable.GetValueOrDefault(false))
                    {
                        context.AddFailure(CartErrorDescriber.ProductUnavailableError(lineItem));
                    }
                    else
                    {
                        var isProductAvailable = new ProductIsAvailableSpecification().IsSatisfiedBy(cartProduct, lineItem.Quantity);
                        if (!isProductAvailable)
                        {
                            var availableQuantity = cartProduct.AvailableQuantity;
                            context.AddFailure(CartErrorDescriber.ProductQtyChangedError(lineItem, availableQuantity));
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
