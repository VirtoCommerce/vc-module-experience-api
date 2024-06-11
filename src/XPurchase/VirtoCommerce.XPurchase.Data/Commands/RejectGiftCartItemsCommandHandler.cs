using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
{
    public class RejectGiftCartItemsCommandHandler : CartCommandHandler<RejectGiftCartItemsCommand>
    {
        public RejectGiftCartItemsCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RejectGiftCartItemsCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            cartAggregate.RejectCartItems(request.Ids);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
