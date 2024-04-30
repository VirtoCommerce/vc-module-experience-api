using FluentValidation;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Validators
{
    public class NewCartItemValidator : AbstractValidator<NewCartItem>
    {
        public NewCartItemValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.ProductId).NotNull();
            RuleFor(x => x.CartProduct).NotNull();

            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((newCartItem, context) =>
                {
                    ValidateCartItem(context, newCartItem);
                });
            });
        }

        /// <summary>
        /// Performs custom validation for the NewCartItem.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="newCartItem"></param>
        protected virtual void ValidateCartItem(ValidationContext<NewCartItem> context, NewCartItem newCartItem)
        {
            if (newCartItem.CartProduct != null)
            {
                var cartProduct = newCartItem.CartProduct;

                VakudateLineItemLimit(context, newCartItem, cartProduct);

                ValidateProductIsBuyable(context, cartProduct);

                ValidateProductInventory(context, cartProduct, newCartItem);

                ValidateMinMaxQuantity(context, cartProduct, newCartItem);
            }
            else
            {
                context.AddFailure(CartErrorDescriber.ProductUnavailableError(nameof(CatalogProduct), newCartItem.ProductId));
            }

            if (newCartItem.Price != null)
            {
                ValidateTierPrice(context, newCartItem);
            }
        }

        protected virtual void VakudateLineItemLimit(ValidationContext<NewCartItem> context, NewCartItem newCartItem, CartProduct cartProduct)
        {
            // LINE_ITEM_LIMIT
            if (newCartItem.Quantity > XPurchaseConstants.LineItemQualityLimit)
            {
                context.AddFailure(CartErrorDescriber.ProductQuantityLimitError(cartProduct.Product, XPurchaseConstants.LineItemQualityLimit));
            }
        }

        protected virtual void ValidateProductIsBuyable(ValidationContext<NewCartItem> context, CartProduct cartProduct)
        {
            if (!AbstractTypeFactory<ProductIsBuyableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct))
            {
                // CART_PRODUCT_UNAVAILABLE
                if (!cartProduct.Product.IsBuyable.GetValueOrDefault(false))
                {
                    context.AddFailure(CartErrorDescriber.ProductUnavailableError(nameof(CatalogProduct), cartProduct?.Product?.Id));
                }

                // CART_PRODUCT_INACTIVE
                if (!cartProduct.Product.IsActive.GetValueOrDefault(false))
                {
                    context.AddFailure(CartErrorDescriber.ProductInactiveError(nameof(CatalogProduct), cartProduct?.Product?.Id));
                }

                // PRODUCT_PRICE_INVALID
                if (cartProduct.Price == null || cartProduct.Price.ListPrice == 0.0)
                {
                    context.AddFailure(CartErrorDescriber.ProductNoPriceError(nameof(CatalogProduct), cartProduct?.Product?.Id));
                }
            }
        }

        /// <summary>
        /// Validates the tier price of the new cart item.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="newCartItem"></param>
        protected virtual void ValidateTierPrice(ValidationContext<NewCartItem> context, NewCartItem newCartItem)
        {
            var productSalePrice = newCartItem.CartProduct.Price.GetTierPrice(newCartItem.Quantity).Price;

            if (newCartItem.Price < productSalePrice.Amount)
            {
                context.AddFailure(CartErrorDescriber.UnableToSetLessPrice(newCartItem.CartProduct));
            }
        }

        /// <summary>
        /// Validates the min and max quantity constraints for the new cart item.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cartProduct"></param>
        /// <param name="newCartItem"></param>
        protected virtual void ValidateMinMaxQuantity(ValidationContext<NewCartItem> context, CartProduct cartProduct, NewCartItem newCartItem)
        {
            var minQuantity = cartProduct?.Product?.MinQuantity;
            var maxQuantity = cartProduct?.Product?.MaxQuantity;

            // PRODUCT_MIN_QTY_NOT_AVAILABLE
            if (IsProductMinQunatityNotAvailable(cartProduct, minQuantity))
            {
                context.AddFailure(CartErrorDescriber.ProductMinQuantityNotAvailableError(cartProduct.Product, minQuantity.Value));
            }

            if (minQuantity.HasValue && minQuantity.Value > 0 && maxQuantity.HasValue && maxQuantity.Value > 0)
            {
                // PRODUCT_MIN_MAX_QTY
                if (ValidationExtensions.IsOutsideMinMaxQuantity(newCartItem.Quantity, minQuantity.Value, maxQuantity.Value))
                {
                    context.AddFailure(CartErrorDescriber.ProductMinMaxQuantityError(cartProduct.Product, newCartItem.Quantity, minQuantity.Value, maxQuantity.Value));
                }
            }
            else
            {
                // PRODUCT_MIN_QTY
                if (ValidationExtensions.IsBelowMinQuantity(newCartItem.Quantity, minQuantity))
                {
                    context.AddFailure(CartErrorDescriber.ProductMinQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Id, newCartItem.Quantity, minQuantity.Value));
                }

                // PRODUCT_MAX_QTY
                if (ValidationExtensions.IsAboveMaxQuantity(newCartItem.Quantity, maxQuantity))
                {
                    context.AddFailure(CartErrorDescriber.ProductMaxQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Id, newCartItem.Quantity, maxQuantity.Value));
                }
            }
        }

        /// <summary>
        /// Validates the product inventory of the new cart item
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cartProduct"></param>
        /// <param name="newCartItem"></param>
        protected virtual void ValidateProductInventory(ValidationContext<NewCartItem> context, CartProduct cartProduct, NewCartItem newCartItem)
        {
            if (!AbstractTypeFactory<ProductIsAvailableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct, newCartItem.Quantity))
            {
                context.AddFailure(CartErrorDescriber.ProductAvailableQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Id, newCartItem.Quantity, cartProduct.AvailableQuantity));
            }
        }

        protected virtual bool IsProductMinQunatityNotAvailable(CartProduct cartProduct, int? minQuantity)
        {
            return !AbstractTypeFactory<ProductMinQunatityAvailableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct, minQuantity);
        }
    }
}
