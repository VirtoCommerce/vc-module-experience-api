using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartAddressCommandHandler : CartCommandHandler<AddOrUpdateCartAddressCommand>
    {
        public AddOrUpdateCartAddressCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(AddOrUpdateCartAddressCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            await cartAggregate.AddOrUpdateCartAddress(request.Address);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
