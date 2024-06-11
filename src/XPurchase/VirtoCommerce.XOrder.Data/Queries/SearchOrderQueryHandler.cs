using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;
using VirtoCommerce.OrdersModule.Core.Search.Indexed;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.XOrder.Core.Queries;
using VirtoCommerce.XOrder.Core.Services;
using VirtoCommerce.XOrder.Data.Services;

namespace VirtoCommerce.XOrder.Data.Queries
{
    public class SearchOrderQueryHandler : IQueryHandler<SearchCustomerOrderQuery, SearchOrderResponse>, IQueryHandler<SearchOrganizationOrderQuery, SearchOrderResponse>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IIndexedCustomerOrderSearchService _customerOrderSearchService;
        private readonly IMapper _mapper;

        public SearchOrderQueryHandler(ISearchPhraseParser searchPhraseParser,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            IIndexedCustomerOrderSearchService customerOrderSearchService,
            IMapper mapper)
        {
            _searchPhraseParser = searchPhraseParser;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
            _customerOrderSearchService = customerOrderSearchService;
            _mapper = mapper;
        }

        public virtual async Task<SearchOrderResponse> Handle(SearchCustomerOrderQuery request, CancellationToken cancellationToken)
        {
            return await SearchOrderQueryHandle(request);
        }

        public virtual async Task<SearchOrderResponse> Handle(SearchOrganizationOrderQuery request, CancellationToken cancellationToken)
        {
            return await SearchOrderQueryHandle(request);
        }

        protected virtual async Task<SearchOrderResponse> SearchOrderQueryHandle(SearchOrderQuery request)
        {
            var searchCriteriaBuilder = new CustomerOrderSearchCriteriaBuilder(_searchPhraseParser)
                                        .WithCultureName(request.CultureName)
                                        .ParseFilters(request.Filter)
                                        .ParseFacets(request.Facet)
                                        .WithPaging(request.Skip, request.Take)
                                        .WithSorting(request.Sort);

            switch (request)
            {
                case SearchCustomerOrderQuery customerOrderQuery:
                    searchCriteriaBuilder = searchCriteriaBuilder.WithCustomerId(customerOrderQuery.CustomerId);
                    break;
                case SearchOrganizationOrderQuery organizationOrderQuery:
                    searchCriteriaBuilder = searchCriteriaBuilder.WithOrganizationId(organizationOrderQuery.OrganizationId);
                    break;
            }

            var searchCriteria = searchCriteriaBuilder.Build();

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
