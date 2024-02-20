using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Queries
{
    public class GetWishlistQueryHandler : IQueryHandler<GetWishlistQuery, CartAggregate>
    {
        private readonly ICartAggregateRepositoryExtended _cartAggrRepository;

        public GetWishlistQueryHandler(ICartAggregateRepositoryExtended cartAggrRepository)
        {
            _cartAggrRepository = cartAggrRepository;
        }

        public Task<CartAggregate> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            return _cartAggrRepository.GetCartByIdAsync(request.ListId, request.IncludeFields.ItemsToProductIncludeField(), language: request.CultureName);
        }
    }
}
