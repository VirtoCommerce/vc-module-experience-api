using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Core.Models;
using VirtoCommerce.XPurchase.Core.Specifications;

namespace VirtoCommerce.XPurchase.Core.Validators
{
    public class ItemQtyAdjustmentValidator : AbstractValidator<ItemQtyAdjustment>
    {
        public ItemQtyAdjustmentValidator()
        {
            RuleFor(x => x.NewQuantity).GreaterThan(0);
            RuleFor(x => x.LineItemId).NotNull();
            RuleFor(x => x.CartProduct).NotNull();

            RuleSet("strict", () =>
            {
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
                        context.AddFailure(CartErrorDescriber.ProductMinQuantityError(qtyAdjust.CartProduct, qtyAdjust.NewQuantity, minQuantity.Value));
                    }

                    var maxQuantity = qtyAdjust.CartProduct?.Product?.MaxQuantity;
                    if (maxQuantity > 0 && qtyAdjust.NewQuantity > maxQuantity)
                    {
                        context.AddFailure(CartErrorDescriber.ProductMaxQuantityError(qtyAdjust.CartProduct, qtyAdjust.NewQuantity, maxQuantity.Value));
                    }
                });
            });
        }
    }
}
