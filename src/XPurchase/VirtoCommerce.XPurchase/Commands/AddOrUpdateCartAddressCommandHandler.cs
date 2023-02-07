using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

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

            if (request.Address.AddressType?.Value == 0)
            {
                request.Address.AddressType = new Optional<int>((int)AddressType.BillingAndShipping);
            }

            var address = cartAggregate.Cart.Addresses.FirstOrDefault(x => x.Key == request.Address.Key?.Value);
            address = request.Address.MapTo(address);

            await cartAggregate.AddOrUpdateCartAddress(address);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
