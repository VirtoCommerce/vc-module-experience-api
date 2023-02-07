using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartAddressCommandHandler : CartCommandHandler<AddCartAddressCommand>
    {
        public AddCartAddressCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(AddCartAddressCommand request, CancellationToken cancellationToken)
        {
            ValidateAddressType(request.Address);
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var address = cartAggregate.Cart.Addresses.FirstOrDefault(x => (int)x.AddressType == request.Address.AddressType?.Value);
            address = request.Address.MapTo(address);

            await cartAggregate.AddOrUpdateCartAddressByTypeAsync(address);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
