using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateCartCommandHandler : CartCommandHandler<CreateCartCommand>
    {
        public CreateCartCommandHandler(ICartAggregateRepository cartAggrRepository)
            :base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await CreateNewCartAggregateAsync(request);
            await CartRepository.SaveAsync(cart);
            return cart;
        }
       
    }
}
