using FluentValidation;
using System.Linq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Validators
{
    public class ItemQtyAdjustmentValidator : AbstractValidator<ItemQtyAdjustment>
    {
        public ItemQtyAdjustmentValidator(CartAggregate cartAggr)
        {
            RuleFor(x => x.NewQuantity).GreaterThan(0);
            RuleFor(x => x.LineItemId).NotNull();
            RuleFor(x => x.CartProduct).NotNull();
            RuleFor(x => x).Custom((qtyAdjust, context) =>
            {
                var lineItem = cartAggr.Cart.Items.FirstOrDefault(x => x.Id.EqualsInvariant(qtyAdjust.LineItemId));
                if (lineItem == null)
                {
                    context.AddFailure(CartErrorDescriber.LineItemWithGivenIdNotFound(new LineItem { Id = qtyAdjust.LineItemId }));
                }
                else if(lineItem.IsReadOnly)
                {
                    context.AddFailure(CartErrorDescriber.LineItemIsReadOnly(lineItem));
                }
            });

        }
    }
}
