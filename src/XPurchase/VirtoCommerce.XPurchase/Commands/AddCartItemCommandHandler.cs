using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Domain.CartAggregate;
using VirtoCommerce.XPurchase.Domain.Factories;

namespace VirtoCommerce.XPurchase.Domain.Commands
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
           await cartAggr.SaveAsync();
        }
    }
}
