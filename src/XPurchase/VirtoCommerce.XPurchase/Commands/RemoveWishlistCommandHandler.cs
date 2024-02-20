using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveWishlistCommandHandler : CartCommandHandler<RemoveWishlistCommand>
    {
        public RemoveWishlistCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RemoveWishlistCommand request, CancellationToken cancellationToken)
        {
            await CartRepository.RemoveCartAsync(request.ListId);
            return null;
        }
    }
}
