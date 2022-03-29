using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveWishlistCommandHandler : IRequestHandler<RemoveWishlistCommand, bool>
    {
        private readonly ICartAggregateRepository _cartAggrRepository;

        public RemoveWishlistCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            _cartAggrRepository = cartAggrRepository;
        }

        public virtual async Task<bool> Handle(RemoveWishlistCommand request, CancellationToken cancellationToken)
        {
            await _cartAggrRepository.RemoveCartAsync(request.ListId);
            return true;
        }
    }
}
