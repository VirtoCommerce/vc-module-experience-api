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
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            await cartAggregate.UpdateCartComment(request.Comment);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
