using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RenameWishlistCommandHandler : IRequestHandler<RenameWishlistCommand, CartAggregate>
    {
        private readonly ICartAggregateRepository _cartRepository;

        public RenameWishlistCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            _cartRepository = cartAggrRepository;
        }

        public virtual async Task<CartAggregate> Handle(RenameWishlistCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await _cartRepository.GetCartByIdAsync(request.ListId);

            cartAggregate.Cart.Name = request.ListName;

            await _cartRepository.SaveAsync(cartAggregate);
            return cartAggregate;
        }
    }
}
