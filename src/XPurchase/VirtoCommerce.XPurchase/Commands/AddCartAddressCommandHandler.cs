using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

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

            if (request.Address.AddressType == null || request.Address.AddressType?.Value == 0)
            {
                request.Address.AddressType = new Optional<int>((int)AddressType.BillingAndShipping);
            }

            var address = cartAggregate.Cart.Addresses.FirstOrDefault(x => (int)x.AddressType == request.Address.AddressType?.Value);
            address = request.Address.MapTo(address);

            await cartAggregate.AddOrUpdateCartAddressByTypeAsync(address);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
