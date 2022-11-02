using FluentValidation;
using VirtoCommerce.CatalogModule.Core.Model;

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
                    if (newCartItem.CartProduct != null)
                    {
                        var cartProduct = newCartItem.CartProduct;

                        // PRODUCT_FFC_QTY
                        if (cartProduct.Product.TrackInventory.GetValueOrDefault(false) && cartProduct.Inventory != null)
                        {
                            var result = cartProduct.Inventory.AllowPreorder ||
                                     cartProduct.Inventory.AllowBackorder ||
                                     cartProduct.AvailableQuantity >= newCartItem.Quantity;

                            if (!result)
                            {
                                context.AddFailure(CartErrorDescriber.ProductAvailableQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Code, newCartItem.Quantity, cartProduct.AvailableQuantity));
                            }
                        }

                        // CART_PRODUCT_UNAVAILABLE
                        if (!cartProduct.Product.IsBuyable.GetValueOrDefault(false))
                        {
                            context.AddFailure(CartErrorDescriber.ProductUnavailableError(nameof(CatalogProduct), cartProduct?.Product?.Code));
                        }

                        // CART_PRODUCT_INACTIVE
                        if (!cartProduct.Product.IsActive.GetValueOrDefault(false))
                        {
                            context.AddFailure(CartErrorDescriber.ProductInactiveError(nameof(CatalogProduct), cartProduct?.Product?.Code));
                        }

                        // PRODUCT_PRICE_INVALID
                        if (cartProduct.Price == null || cartProduct.Price.ListPrice == 0.0)
                        {
                            context.AddFailure(CartErrorDescriber.ProductNoPriceError(nameof(CatalogProduct), cartProduct?.Product?.Code));
                        }

                        // PRODUCT_MIN_QTY
                        var minQuantity = cartProduct?.Product?.MinQuantity;
                        if (newCartItem.Quantity < minQuantity)
                        {
                            context.AddFailure(CartErrorDescriber.ProductMinQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Code, newCartItem.Quantity, minQuantity ?? 0));
                        }

                        // PRODUCT_MAX_QTY
                        var maxQuantity = cartProduct?.Product?.MaxQuantity;
                        if (maxQuantity > 0 && newCartItem.Quantity > maxQuantity)
                        {
                            context.AddFailure(CartErrorDescriber.ProductMaxQuantityError(nameof(CatalogProduct), cartProduct?.Product?.Code, newCartItem.Quantity, maxQuantity ?? 0));
                        }
                    }
                    if (newCartItem.Price != null)
                    {
                        var productSalePrice = newCartItem.CartProduct.Price.GetTierPrice(newCartItem.Quantity).Price;

                        if (newCartItem.Price < productSalePrice.Amount)
                        {
                            context.AddFailure(CartErrorDescriber.UnableToSetLessPrice(newCartItem.CartProduct));
                        }
                    }
                });
            });
        }
    }
}
