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
            await cartAggregate.ChangeItemPriceAsync(new PriceAdjustment(request.LineItemId, request.Price));

            return await SaveCartAsync(cartAggregate);
        }
    }
}
