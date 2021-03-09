using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartItemsCommandHandler : CartCommandHandler<AddCartItemsCommand>
    {
        public AddCartItemsCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(AddCartItemsCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            await cartAggregate.AddItemsAsync(request.AddCartItems);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
