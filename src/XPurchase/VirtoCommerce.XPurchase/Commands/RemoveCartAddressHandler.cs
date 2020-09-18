using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCartAddressHandler : CartCommandHandler<RemoveCartAddressCommand>
    {
        public RemoveCartAddressHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RemoveCartAddressCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            await cartAggregate.RemoveCartAddress(new CartModule.Core.Model.Address { Key = request.AddressId });

            return await SaveCartAsync(cartAggregate);
        }
    }
}
