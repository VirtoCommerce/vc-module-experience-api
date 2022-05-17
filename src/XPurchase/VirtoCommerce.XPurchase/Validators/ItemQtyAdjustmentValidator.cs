using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Validators
{
    public class ItemQtyAdjustmentValidator : AbstractValidator<ItemQtyAdjustment>
    {
        public ItemQtyAdjustmentValidator()
        {
            RuleFor(x => x.NewQuantity).GreaterThan(0);
            RuleFor(x => x.LineItemId).NotNull();
            RuleFor(x => x.CartProduct).NotNull();
            RuleFor(x => x).Custom((qtyAdjust, context) =>
            {
                if (qtyAdjust.LineItem == null)
                {
                    context.AddFailure(CartErrorDescriber.LineItemWithGivenIdNotFound(new LineItem { Id = qtyAdjust.LineItemId }));
                }
                else if (qtyAdjust.LineItem.IsReadOnly || qtyAdjust.LineItem.IsGift)
                {
                    context.AddFailure(CartErrorDescriber.LineItemIsReadOnly(qtyAdjust.LineItem));
                }

                if (qtyAdjust.CartProduct != null && !new ProductIsAvailableSpecification().IsSatisfiedBy(qtyAdjust.CartProduct, qtyAdjust.NewQuantity))
                {
                    context.AddFailure(CartErrorDescriber.ProductQtyInsufficientError(qtyAdjust.CartProduct, qtyAdjust.NewQuantity, qtyAdjust.CartProduct.AvailableQuantity));
                }

                var minQuantity = qtyAdjust.CartProduct?.Product?.MinQuantity;
                if (qtyAdjust.NewQuantity < minQuantity)
                {
                    context.AddFailure(CartErrorDescriber.ProductMinQuantityError(qtyAdjust.CartProduct, qtyAdjust.NewQuantity, minQuantity ?? 0));
                }

                var maxQuantity = qtyAdjust.CartProduct?.Product?.MaxQuantity;
                if (qtyAdjust.NewQuantity > maxQuantity)
                {
                    context.AddFailure(CartErrorDescriber.ProductMaxQuantityError(qtyAdjust.CartProduct, qtyAdjust.NewQuantity, maxQuantity ?? 0));
                }
            });
        }
    }
}
