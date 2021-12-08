using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Queries
{
    public class GetWishlistQueryHandler : IQueryHandler<GetWishlistQuery, CartAggregate>
    {
        private readonly ICartAggregateRepository _cartAggrRepository;

        public GetWishlistQueryHandler(ICartAggregateRepository cartAggrRepository)
        {
            _cartAggrRepository = cartAggrRepository;
        }

        public Task<CartAggregate> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            return _cartAggrRepository.GetCartByIdAsync(request.ListId);
        }
    }
}
