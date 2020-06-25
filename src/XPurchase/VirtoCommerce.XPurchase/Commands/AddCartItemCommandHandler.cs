using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartItemCommandHandler : CartCommandHandler<AddCartItemCommand>
    {
        public AddCartItemCommandHandler(ICartAggregateRepository cartRepository)
            :base(cartRepository)
        {
        }
        protected override async Task Handle(AddCartItemCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetCartAggregateFromCommandAsync(request);
            await cartAggr.AddItemAsync(new NewCartItem(request.ProductId, request.Quantity)
            {
                Comment = request.Comment,
                DynamicProperties = request.DynamicProperties,
                Price = request.Price
            });
            await CartAggrRepository.SaveAsync(cartAggr);
        }
    }
}
