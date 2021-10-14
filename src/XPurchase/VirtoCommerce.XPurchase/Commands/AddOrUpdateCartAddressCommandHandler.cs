using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartAddressCommandHandler : CartCommandHandler<AddOrUpdateCartAddressCommand>
    {
        private readonly IMapper _mapper;

        public AddOrUpdateCartAddressCommandHandler(ICartAggregateRepository cartRepository, IMapper mapper)
            : base(cartRepository)
        {
            _mapper = mapper;
        }

        public override async Task<CartAggregate> Handle(AddOrUpdateCartAddressCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var address = cartAggregate.Cart.Addresses.FirstOrDefault(x => x.Key == request.Address.Key?.Value);
            address = _mapper.Map(request.Address, address);

            await cartAggregate.AddOrUpdateCartAddress(address);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
