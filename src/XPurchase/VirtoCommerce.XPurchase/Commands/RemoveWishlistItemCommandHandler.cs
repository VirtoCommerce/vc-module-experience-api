using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveWishlistItemCommandHandler : IRequestHandler<RemoveWishlistItemCommand, CartAggregate>
    {
        private readonly ICartAggregateRepository _cartRepository;

        public RemoveWishlistItemCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            _cartRepository = cartAggrRepository;
        }

        public virtual async Task<CartAggregate> Handle(RemoveWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await _cartRepository.GetCartByIdAsync(request.ListId);

            await cartAggregate.RemoveItemAsync(request.LineItemId);

            await _cartRepository.SaveAsync(cartAggregate);
            return cartAggregate;
        }
    }
}
