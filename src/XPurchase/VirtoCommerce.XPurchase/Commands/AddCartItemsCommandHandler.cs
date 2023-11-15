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

            if (request.ValidationRuleSet != null)
            {
                cartAggregate.ValidationRuleSet = request.ValidationRuleSet;
            }

            await cartAggregate.AddItemsAsync(request.CartItems);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
