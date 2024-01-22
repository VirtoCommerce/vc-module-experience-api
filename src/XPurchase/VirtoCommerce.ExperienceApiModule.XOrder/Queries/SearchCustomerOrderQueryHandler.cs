using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;
using VirtoCommerce.OrdersModule.Core.Search.Indexed;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchCustomerOrderQueryHandler : IQueryHandler<SearchCustomerOrderQuery, SearchOrderResponse>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IIndexedCustomerOrderSearchService _customerOrderSearchService;

        public SearchCustomerOrderQueryHandler(ISearchPhraseParser searchPhraseParser,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            IIndexedCustomerOrderSearchService customerOrderSearchService)
        {
            _searchPhraseParser = searchPhraseParser;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
            _customerOrderSearchService = customerOrderSearchService;
        }

        public virtual async Task<SearchOrderResponse> Handle(SearchCustomerOrderQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new CustomerOrderSearchCriteriaBuilder(_searchPhraseParser)
                                        .WithCultureName(request.CultureName)
                                        .ParseFilters(request.Filter)
                                        .ParseFacets(request.Facet)
                                        .WithCustomerId(request.CustomerId)
                                        .WithPaging(request.Skip, request.Take)
                                        .WithSorting(request.Sort)
                                        .Build();

            var searchResult = await _customerOrderSearchService.SearchCustomerOrdersAsync(searchCriteria);
            var aggregates = await _customerOrderAggregateRepository.GetAggregatesFromOrdersAsync(searchResult.Results, request.CultureName);

            var facets = searchResult.Aggregations?.Select(request => new TermFacetResult
            {
                Name = request.Field,
                Label = request.Field,
                Terms = request.Items?.Select(x => new FacetTerm
                {
                    Count = x.Count,
                    IsSelected = x.IsApplied,
                    Term = x.Value?.ToString(),
                    Label = x.Value.ToString(),
                }).ToArray() ?? [],
            }).Cast<FacetResult>().ToList();

            return new SearchOrderResponse
            {
                TotalCount = searchResult.TotalCount,
                Results = aggregates,
                Facets = facets,
            };
        }
    }
}
