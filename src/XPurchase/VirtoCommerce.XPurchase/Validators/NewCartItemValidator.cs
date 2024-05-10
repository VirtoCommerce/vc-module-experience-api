using FluentValidation;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Specifications;

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

                if (ValidateLineItemLimit(context, newCartItem, cartProduct)
                    && ValidateProductIsBuyable(context, cartProduct)
                    && ValidateProductInStock(context, cartProduct, newCartItem)
                    && ValidateMinQuantity(context, cartProduct))
                {
                    ValidateMinMaxQuantity(context, cartProduct, newCartItem);
                }
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

        protected virtual bool ValidateLineItemLimit(ValidationContext<NewCartItem> context, NewCartItem newCartItem, CartProduct cartProduct)
        {
            // LINE_ITEM_LIMIT
            if (newCartItem.Quantity > XPurchaseConstants.LineItemQualityLimit)
            {
                context.AddFailure(CartErrorDescriber.ProductQuantityLimitError(cartProduct.Product, XPurchaseConstants.LineItemQualityLimit));
                return false;
            }

            return true;
        }

        private bool ValidateMinQuantity(ValidationContext<NewCartItem> context, CartProduct cartProduct)
        {
            var minQuantity = cartProduct.GetMinQuantity();

            if (IsProductMinQunatityNotAvailable(cartProduct, minQuantity))
            {
                // PRODUCT_MIN_QTY_NOT_AVAILABLE
                context.AddFailure(CartErrorDescriber.ProductMinQuantityNotAvailableError(cartProduct.Product, minQuantity ?? 0));
                return false;
            }

            return true;
        }

        protected virtual bool ValidateProductIsBuyable(ValidationContext<NewCartItem> context, CartProduct cartProduct)
        {
            if (!AbstractTypeFactory<ProductIsBuyableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct))
            {
                // CART_PRODUCT_UNAVAILABLE
                if (!cartProduct.Product.IsBuyable.GetValueOrDefault(false))
                {
                    context.AddFailure(CartErrorDescriber.ProductUnavailableError(nameof(CatalogProduct), cartProduct.Product.Id));
                }

                // CART_PRODUCT_INACTIVE
                if (!cartProduct.Product.IsActive.GetValueOrDefault(false))
                {
                    context.AddFailure(CartErrorDescriber.ProductInactiveError(nameof(CatalogProduct), cartProduct.Product.Id));
                }

                // PRODUCT_PRICE_INVALID
                if (cartProduct.Price == null || cartProduct.Price.ListPrice == 0.0)
                {
                    context.AddFailure(CartErrorDescriber.ProductNoPriceError(nameof(CatalogProduct), cartProduct.Product.Id));
                }

                return false;
            }

            return true;
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
            var minQuantity = cartProduct.GetMinQuantity();
            var maxQuantity = cartProduct.GetMaxQuantity();

            if (minQuantity.HasValue && maxQuantity.HasValue)
            {
                if (ValidationExtensions.IsOutsideMinMaxQuantity(newCartItem.Quantity, minQuantity.Value, maxQuantity.Value))
                {
                    // PRODUCT_MIN_MAX_QTY
                    context.AddFailure(CartErrorDescriber.ProductMinMaxQuantityError(cartProduct.Product, newCartItem.Quantity, minQuantity.Value, maxQuantity.Value));
                }
            }
            else if (ValidationExtensions.IsBelowMinQuantity(newCartItem.Quantity, minQuantity))
            {
                // PRODUCT_MIN_QTY
                context.AddFailure(CartErrorDescriber.ProductMinQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Id, newCartItem.Quantity, minQuantity ?? 0));
            }
            else if (ValidationExtensions.IsAboveMaxQuantity(newCartItem.Quantity, maxQuantity))
            {
                // PRODUCT_MAX_QTY
                context.AddFailure(CartErrorDescriber.ProductMaxQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Id, newCartItem.Quantity, maxQuantity ?? 0));
            }
            else
            {
                ValidateProductInventory(context, cartProduct, newCartItem);
            }
        }

        /// <summary>
        /// Validates the product inventory of the new cart item
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cartProduct"></param>
        /// <param name="newCartItem"></param>
        protected virtual bool ValidateProductInventory(ValidationContext<NewCartItem> context, CartProduct cartProduct, NewCartItem newCartItem)
        {
            if (!AbstractTypeFactory<ProductIsAvailableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct, newCartItem.Quantity))
            {
                // PRODUCT_FFC_QTY
                context.AddFailure(CartErrorDescriber.ProductAvailableQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Id, newCartItem.Quantity, cartProduct.AvailableQuantity));
                return false;
            }

            return true;
        }

        protected virtual bool ValidateProductInStock(ValidationContext<NewCartItem> context, CartProduct cartProduct, NewCartItem newCartItem)
        {
            if (!AbstractTypeFactory<ProductIsInStockSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct))
            {
                // PRODUCT_FFC_QTY
                context.AddFailure(CartErrorDescriber.ProductAvailableQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Id, newCartItem.Quantity, cartProduct.AvailableQuantity));
                return false;
            }

            return true;
        }

        protected virtual bool IsProductMinQunatityNotAvailable(CartProduct cartProduct, int? minQuantity)
        {
            return !AbstractTypeFactory<ProductMinQunatityAvailableSpecification>.TryCreateInstance().IsSatisfiedBy(cartProduct, minQuantity);
        }
    }
}
