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
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            await cartAggregate.AddOrUpdateCartAddressByTypeAsync(request.Address);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
