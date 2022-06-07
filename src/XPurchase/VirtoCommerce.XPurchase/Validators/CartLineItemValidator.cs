using System.Linq;
using FluentValidation;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartLineItemValidator : AbstractValidator<LineItemValidationContext>
    {
        public CartLineItemValidator()
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
                    if (tierPrice.Price.Amount != lineItem.SalePrice)
                    {
                        context.AddFailure(CartErrorDescriber.ProductPriceChangedError(lineItem, lineItem.SalePrice, lineItem.SalePriceWithTax, tierPrice.Price.Amount, tierPrice.PriceWithTax.Amount));
                    }

                    var minQuantity = cartProduct?.Product?.MinQuantity;
                    if (lineItem.Quantity < minQuantity)
                    {
                        context.AddFailure(CartErrorDescriber.ProductMinQuantityError(lineItem, lineItem.Quantity, minQuantity ?? 0));
                    }

                    var maxQuantity = cartProduct?.Product?.MaxQuantity;
                    if (maxQuantity > 0 && lineItem.Quantity > maxQuantity)
                    {
                        context.AddFailure(CartErrorDescriber.ProductMaxQuantityError(lineItem, lineItem.Quantity, maxQuantity ?? 0));
                    }
                }
            });
        }
    }
}
