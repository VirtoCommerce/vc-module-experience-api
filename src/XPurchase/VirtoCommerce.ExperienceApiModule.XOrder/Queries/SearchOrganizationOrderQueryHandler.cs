using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;
using VirtoCommerce.OrdersModule.Core.Search.Indexed;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrganizationOrderQueryHandler : IQueryHandler<SearchOrganizationOrderQuery, SearchOrderResponse>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IIndexedCustomerOrderSearchService _customerOrderSearchService;
        private readonly IMapper _mapper;

        public SearchOrganizationOrderQueryHandler(ISearchPhraseParser searchPhraseParser,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            IIndexedCustomerOrderSearchService customerOrderSearchService,
            IMapper mapper)
        {
            _searchPhraseParser = searchPhraseParser;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
            _customerOrderSearchService = customerOrderSearchService;
            _mapper = mapper;
        }

        public virtual async Task<SearchOrderResponse> Handle(SearchOrganizationOrderQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new CustomerOrderSearchCriteriaBuilder(_searchPhraseParser)
                                        .ParseFilters(request.Filter)
                                        .WithOrganizationId(request.OrganizationId)
                                        .WithPaging(request.Skip, request.Take)
                                        .WithSorting(request.Sort)
                                        .Build();

            var searchResult = await _customerOrderSearchService.SearchCustomerOrdersAsync(searchCriteria);
            var aggregates = await _customerOrderAggregateRepository.GetAggregatesFromOrdersAsync(searchResult.Results, request.CultureName);

            var facets = searchResult.Aggregations?.Select(x => _mapper.Map<FacetResult>(x, options =>
            {
                options.Items["cultureName"] = request.CultureName;
            })).ToList() ?? [];

            return new SearchOrderResponse
            {
                TotalCount = searchResult.TotalCount,
                Results = aggregates,
                Facets = facets,
            };
        }
    }
}
