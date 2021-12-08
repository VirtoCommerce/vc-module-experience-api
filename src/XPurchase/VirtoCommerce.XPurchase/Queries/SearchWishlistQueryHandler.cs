using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XPurchase.Queries
{
    public class SearchWishlistQueryHandler : IQueryHandler<SearchWishlistQuery, SearchCartResponse>
    {
        private readonly ICartAggregateRepository _cartAggregateRepository;
        private readonly IMapper _mapper;
        private readonly ISearchPhraseParser _searchPhraseParser;

        public SearchWishlistQueryHandler(
            ICartAggregateRepository cartAggregateRepository
            , IMapper mapper
            , ISearchPhraseParser searchPhraseParser)
        {
            _cartAggregateRepository = cartAggregateRepository;
            _mapper = mapper;
            _searchPhraseParser = searchPhraseParser;
        }

        public virtual Task<SearchCartResponse> Handle(SearchWishlistQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new CartSearchCriteriaBuilder(_searchPhraseParser, _mapper)
                                     .WithCurrency(request.CurrencyCode)
                                     .WithStore(request.StoreId)
                                     .WithType(XPurchaseConstants.ListTypeName)
                                     .WithLanguage(request.CultureName)
                                     .WithCustomerId(request.UserId)
                                     .WithPaging(request.Skip, request.Take)
                                     .WithSorting(request.Sort)
                                     .Build();

            searchCriteria.ResponseGroup = CartResponseGroup.WithLineItems.ToString();
            return _cartAggregateRepository.SearchCartAsync(searchCriteria);
        }
    }
}
