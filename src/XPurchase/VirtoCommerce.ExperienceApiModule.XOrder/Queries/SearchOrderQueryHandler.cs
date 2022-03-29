using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.OrdersModule.Core.Search.Indexed;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrderQueryHandler : IQueryHandler<SearchOrderQuery, SearchOrderResponse>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IIndexedCustomerOrderSearchService _customerOrderSearchService;

        public SearchOrderQueryHandler(ISearchPhraseParser searchPhraseParser,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            IIndexedCustomerOrderSearchService customerOrderSearchService)
        {
            _searchPhraseParser = searchPhraseParser;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
            _customerOrderSearchService = customerOrderSearchService;
        }

        public virtual async Task<SearchOrderResponse> Handle(SearchOrderQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new CustomerOrderSearchCriteriaBuilder(_searchPhraseParser)
                                        .ParseFilters(request.Filter)
                                        .WithCustomerId(request.CustomerId)
                                        .WithPaging(request.Skip, request.Take)
                                        .WithSorting(request.Sort)
                                        .Build();
            var searchResult = await _customerOrderSearchService.SearchCustomerOrdersAsync(searchCriteria);
            var aggregates = await _customerOrderAggregateRepository.GetAggregatesFromOrdersAsync(searchResult.Results, request.CultureName);
            return new SearchOrderResponse { TotalCount = searchResult.TotalCount, Results = aggregates };
        }
    }
}
