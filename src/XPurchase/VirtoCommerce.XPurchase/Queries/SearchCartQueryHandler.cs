using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Queries
{
    public class SearchCartQueryHandler : IQueryHandler<SearchCartQuery, SearchCartResponse>
    {
        private readonly ICartAggregateRepository _cartAggregateRepository;

        public SearchCartQueryHandler(ICartAggregateRepository cartAggregateRepository)
        {
            _cartAggregateRepository = cartAggregateRepository;
        }

        public Task<SearchCartResponse> Handle(SearchCartQuery request, CancellationToken cancellationToken)
        {
            return _cartAggregateRepository.SearchCartAsync(request.StoreId, request.UserId, request.CultureName, request.CurrencyCode, request.CartType, request.Sort, request.Skip, request.Take);
        }
    }
}
