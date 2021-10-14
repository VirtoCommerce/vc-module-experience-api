using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartAddressCommandHandler : CartCommandHandler<AddCartAddressCommand>
    {
        private readonly IMapper _mapper;

        public AddCartAddressCommandHandler(ICartAggregateRepository cartRepository, IMapper mapper)
            : base(cartRepository)
        {
            _mapper = mapper;
        }

        public override async Task<CartAggregate> Handle(AddCartAddressCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var address = cartAggregate.Cart.Addresses.FirstOrDefault(x => (int)x.AddressType == request.Address.AddressType?.Value);
            address = _mapper.Map(request.Address, address);

            await cartAggregate.AddOrUpdateCartAddressByTypeAsync(address);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
