using FluentValidation;

namespace VirtoCommerce.XPurchase.Validators
{

    /// <summary>
    /// Represents a cart lineitem price changed validator which transformed to cart warning.
    /// </summary>
    public class CartLineItemPriceChangedValidator : AbstractValidator<CartLineItemPriceChangedValidationContext>
    {
        public CartLineItemPriceChangedValidator()
        {
            RuleFor(x => x).Custom((lineItemContext, context) =>
            {
                var lineItem = lineItemContext.LineItem;

                if (lineItemContext.CartProducts.TryGetValue(lineItem.ProductId, out var cartProduct) &&
                    cartProduct?.Price != null)
                {
                    var tierPrice = cartProduct.Price.GetTierPrice(lineItem.Quantity);
                    if (tierPrice.ActualPrice.Amount > 0 && tierPrice.ActualPrice.Amount != lineItem.SalePrice)
                    {
                        context.AddFailure(CartErrorDescriber.ProductPriceChangedError(lineItem, lineItem.SalePrice, lineItem.SalePriceWithTax, tierPrice.ActualPrice.Amount, tierPrice.ActualPriceWithTax.Amount));
                    }
                }
            });
        }
    }
}
