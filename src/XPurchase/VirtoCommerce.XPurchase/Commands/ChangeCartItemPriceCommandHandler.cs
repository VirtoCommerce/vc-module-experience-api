using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemPriceCommandHandler : CartCommandHandler<ChangeCartItemPriceCommand>
    {
        public ChangeCartItemPriceCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeCartItemPriceCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            if (cartAggregate == null)
            {
                throw new OperationCanceledException("cart not found");
            }
            var lineItem = cartAggregate.Cart.Items.FirstOrDefault(x => x.Id.Equals(request.LineItemId));
            var priceAdjustment = new PriceAdjustment
            {
                LineItem = lineItem,
                LineItemId = request.LineItemId,
                NewPrice = request.Price
            };
            await cartAggregate.ChangeItemPriceAsync(priceAdjustment);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
