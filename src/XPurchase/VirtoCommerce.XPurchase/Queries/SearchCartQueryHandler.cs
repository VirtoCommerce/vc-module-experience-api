using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XPurchase.Queries
{
    public class SearchCartQueryHandler : IQueryHandler<SearchCartQuery, SearchCartResponse>
    {
        private readonly ICartAggregateRepository _cartAggregateRepository;
        private readonly IMapper _mapper;
        private readonly ISearchPhraseParser _searchPhraseParser;

        public SearchCartQueryHandler(
            ICartAggregateRepository cartAggregateRepository
            , IMapper mapper
            , ISearchPhraseParser searchPhraseParser)
        {
            _cartAggregateRepository = cartAggregateRepository;
            _mapper = mapper;
            _searchPhraseParser = searchPhraseParser;
        }

        public Task<SearchCartResponse> Handle(SearchCartQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new CartSearchCriteriaBuilder(_searchPhraseParser, _mapper)
                                     .ParseFilters(request.Filter)
                                     .WithCurrency(request.CurrencyCode)
                                     .WithStore(request.StoreId)
                                     .WithType(request.CartType)
                                     .WithLanguage(request.CultureName)
                                     .WithCustomerId(request.UserId)
                                     .WithPaging(request.Skip, request.Take)
                                     .WithSorting(request.Sort)
                                     .Build();

            return _cartAggregateRepository.SearchCartAsync(searchCriteria);
        }
    }
}
