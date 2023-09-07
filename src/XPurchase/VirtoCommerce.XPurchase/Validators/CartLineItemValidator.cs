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
                if (IsProductNotBuyable(cartProduct))
                {
                    context.AddFailure(CartErrorDescriber.ProductUnavailableError(lineItem));
                }
                else
                {
                    if (IsProductNotAvailable(cartProduct, lineItem.Quantity))
                    {
                        context.AddFailure(CartErrorDescriber.ProductQtyChangedError(lineItem, cartProduct.AvailableQuantity));
                    }

                    var minQuantity = cartProduct?.Product?.MinQuantity;
                    if (IsBelowMinQuantity(lineItem.Quantity, minQuantity))
                    {
                        context.AddFailure(CartErrorDescriber.ProductMinQuantityError(lineItem, lineItem.Quantity, minQuantity ?? 0));
                    }

                    var maxQuantity = cartProduct?.Product?.MaxQuantity;
                    if (IsAboveMaxQuantity(lineItem.Quantity, maxQuantity))
                    {
                        context.AddFailure(CartErrorDescriber.ProductMaxQuantityError(lineItem, lineItem.Quantity, maxQuantity ?? 0));
                    }
                }
            });
        }

        protected virtual bool IsProductNotBuyable(CartProduct cartProduct)
        {
            return cartProduct is null || !AbstractTypeFactory<ProductIsBuyableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct);
        }

        protected virtual bool IsProductNotAvailable(CartProduct cartProduct, int quantity)
        {
            return !AbstractTypeFactory<ProductIsAvailableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct, quantity);
        }

        protected virtual bool IsBelowMinQuantity(int quantity, int? minQuantity)
        {
            return minQuantity.HasValue && minQuantity.Value > 0 && quantity < minQuantity.Value;
        }

        protected virtual bool IsAboveMaxQuantity(int quantity, int? maxQuantity)
        {
            return maxQuantity.HasValue && maxQuantity.Value > 0 && quantity > maxQuantity.Value;
        }
    }

}
