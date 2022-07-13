using FluentValidation;

namespace VirtoCommerce.XPurchase.Validators
{

    public class CartLineItemPriceChangedValidator : AbstractValidator<CartLineItemPriceChangedValidationContext>
    {
        public CartLineItemPriceChangedValidator()
        {
            RuleFor(x => x).Custom((lineItemContext, context) =>
            {
                var lineItem = lineItemContext.LineItem;

                if (lineItemContext.CartProducts.TryGetValue(lineItem.ProductId, out var cartProduct))
                {
                    var tierPrice = cartProduct.Price.GetTierPrice(lineItem.Quantity);
                    if (tierPrice.Price.Amount != lineItem.SalePrice)
                    {
                        context.AddFailure(CartErrorDescriber.ProductPriceChangedError(lineItem, lineItem.SalePrice, lineItem.SalePriceWithTax, tierPrice.Price.Amount, tierPrice.PriceWithTax.Amount));
                    }
                }
            });
        }
    }
}
