using FluentValidation;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

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

                ValidateProductIsBuyable(context, cartProduct);

                ValidateProductInventory(context, cartProduct, newCartItem);

                ValidateMinMaxQuantity(context, cartProduct, newCartItem);
            }

            if (newCartItem.Price != null)
            {
                ValidateTierPrice(context, newCartItem);
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
            // PRODUCT_MIN_QTY
            var minQuantity = cartProduct?.Product?.MinQuantity;
            if (newCartItem.Quantity < minQuantity)
            {
                context.AddFailure(CartErrorDescriber.ProductMinQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Id, newCartItem.Quantity, minQuantity.Value));
            }

            // PRODUCT_MAX_QTY
            var maxQuantity = cartProduct?.Product?.MaxQuantity;
            if (maxQuantity > 0 && newCartItem.Quantity > maxQuantity)
            {
                context.AddFailure(CartErrorDescriber.ProductMaxQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Id, newCartItem.Quantity, maxQuantity.Value));
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
    }
}
