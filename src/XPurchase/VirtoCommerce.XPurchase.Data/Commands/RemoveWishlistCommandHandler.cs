using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
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
