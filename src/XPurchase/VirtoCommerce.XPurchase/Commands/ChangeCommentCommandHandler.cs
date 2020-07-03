using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCommentCommandHandler : CartCommandHandler<ChangeCommentCommand>
    {
        public ChangeCommentCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeCommentCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetOrCreateCartFromCommandAsync(request);
            await cartAggr.UpdateCartComment(request.Comment);
            await CartRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
    }
}
