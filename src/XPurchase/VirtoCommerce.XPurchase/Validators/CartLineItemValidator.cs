using System.Linq;
using FluentValidation;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Extensions;

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

                if (lineItemContext.LineItem.Quantity > XPurchaseConstants.LineItemQualityLimit)
                {
                    // LINE_ITEM_LIMIT
                    context.AddFailure(CartErrorDescriber.ProductQuantityLimitError(lineItemContext.LineItem, XPurchaseConstants.LineItemQualityLimit));
                }
                else if (IsProductNotBuyable(cartProduct))
                {
                    // CART_PRODUCT_UNAVAILABLE
                    context.AddFailure(CartErrorDescriber.ProductUnavailableError(lineItem));
                }
                else
                {
                    var minQuantity = cartProduct.GetMinQuantity();
                    var maxQuantity = cartProduct.GetMaxQuantity();

                    if (minQuantity.HasValue && maxQuantity.HasValue)
                    {
                        // PRODUCT_MIN_MAX_QTY
                        if (IsOutsideMinMaxQuantity(lineItem.Quantity, minQuantity.Value, maxQuantity.Value))
                        {
                            context.AddFailure(CartErrorDescriber.ProductMinMaxQuantityError(lineItem, lineItem.Quantity, minQuantity.Value, maxQuantity.Value));
                        }
                    }
                    else if (IsBelowMinQuantity(lineItem.Quantity, minQuantity))
                    {
                        context.AddFailure(CartErrorDescriber.ProductMinQuantityError(lineItem, lineItem.Quantity, minQuantity ?? 0));
                    }
                    else if (IsAboveMaxQuantity(lineItem.Quantity, maxQuantity))
                    {
                        context.AddFailure(CartErrorDescriber.ProductMaxQuantityError(lineItem, lineItem.Quantity, maxQuantity ?? 0));
                    }
                    else if (IsProductNotAvailable(cartProduct, lineItem.Quantity))
                    {
                        context.AddFailure(CartErrorDescriber.ProductQtyChangedError(lineItem, cartProduct.AvailableQuantity));
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

        protected virtual bool IsProductMinQunatityNotAvailable(CartProduct cartProduct, int? minQuantity)
        {
            return !AbstractTypeFactory<ProductMinQunatityAvailableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct, minQuantity);
        }

        protected virtual bool IsOutsideMinMaxQuantity(int quantity, int minQuantity, int maxQuantity)
        {
            return ValidationExtensions.IsOutsideMinMaxQuantity(quantity, minQuantity, maxQuantity);
        }

        protected virtual bool IsBelowMinQuantity(int quantity, int? minQuantity)
        {
            return ValidationExtensions.IsBelowMinQuantity(quantity, minQuantity);
        }

        protected virtual bool IsAboveMaxQuantity(int quantity, int? maxQuantity)
        {
            return ValidationExtensions.IsAboveMaxQuantity(quantity, maxQuantity);
        }
    }
}
