using System.Linq;
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

            var address = cartAggregate.Cart.Addresses.FirstOrDefault(x => x.Key == request.Address.Key?.Value);
            address = request.Address.MapTo(address);

            await cartAggregate.AddOrUpdateCartAddress(address);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
