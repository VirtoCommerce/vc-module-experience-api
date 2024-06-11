using System.Linq;
using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Core.Extensions;
using VirtoCommerce.XPurchase.Core.Models;
using VirtoCommerce.XPurchase.Core.Specifications;

namespace VirtoCommerce.XPurchase.Core.Validators
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

                var minQuantity = cartProduct?.GetMinQuantity();

                if (lineItemContext.LineItem.Quantity > ModuleConstants.LineItemQualityLimit)
                {
                    // LINE_ITEM_LIMIT
                    context.AddFailure(CartErrorDescriber.ProductQuantityLimitError(lineItemContext.LineItem, ModuleConstants.LineItemQualityLimit));
                }
                else if (IsProductNotBuyable(cartProduct))
                {
                    // CART_PRODUCT_UNAVAILABLE
                    context.AddFailure(CartErrorDescriber.ProductUnavailableError(lineItem));
                }
                else if (IsProductNotInStock(cartProduct))
                {
                    // PRODUCT_FFC_QTY
                    context.AddFailure(CartErrorDescriber.ProductAvailableQuantityError(lineItem, lineItem.Quantity, 0));
                }
                else if (IsProductMinQunatityNotAvailable(cartProduct, minQuantity))
                {
                    // PRODUCT_MIN_QTY_NOT_AVAILABLE
                    context.AddFailure(CartErrorDescriber.ProductMinQuantityNotAvailableError(lineItem, minQuantity ?? 0));
                }
                else
                {
                    ValidateMinMaxQuantity(context, lineItem, cartProduct);
                }
            });
        }

        private void ValidateMinMaxQuantity(ValidationContext<LineItemValidationContext> context, LineItem lineItem, CartProduct cartProduct)
        {
            var minQuantity = cartProduct?.GetMinQuantity();
            var maxQuantity = cartProduct?.GetMaxQuantity();

            if (minQuantity.HasValue && maxQuantity.HasValue)
            {
                if (IsOutsideMinMaxQuantity(lineItem.Quantity, minQuantity.Value, maxQuantity.Value))
                {
                    if (minQuantity.Value == maxQuantity.Value)
                    {
                        // PRODUCT_EXACT_QTY
                        context.AddFailure(CartErrorDescriber.ProductExactQuantityError(lineItem, lineItem.Quantity, minQuantity.Value));
                    }
                    else
                    {
                        // PRODUCT_MIN_MAX_QTY
                        context.AddFailure(CartErrorDescriber.ProductMinMaxQuantityError(lineItem, lineItem.Quantity, minQuantity.Value, maxQuantity.Value));
                    }
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

        protected virtual bool IsProductNotBuyable(CartProduct cartProduct)
        {
            return cartProduct is null || !AbstractTypeFactory<ProductIsBuyableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct);
        }

        protected virtual bool IsProductNotAvailable(CartProduct cartProduct, int quantity)
        {
            return !AbstractTypeFactory<ProductIsAvailableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct, quantity);
        }

        protected virtual bool IsProductNotInStock(CartProduct cartProduct)
        {
            return !AbstractTypeFactory<ProductIsInStockSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct);
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
