using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrderQueryHandler : IQueryHandler<SearchOrderQuery, SearchOrderResponse>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly IMapper _mapper;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly ICustomerOrderSearchService _customerOrderSearchService;

        public SearchOrderQueryHandler(IMapper mapper,
            ISearchPhraseParser searchPhraseParser,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            ICustomerOrderSearchService customerOrderSearchService)
        {
            _mapper = mapper;
            _searchPhraseParser = searchPhraseParser;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
            _customerOrderSearchService = customerOrderSearchService;
        }

        public async Task<SearchOrderResponse> Handle(SearchOrderQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new CustomerOrderSearchCriteriaBuilder(_searchPhraseParser, _mapper)
                                        .ParseFilters(request.Filter)
                                        .WithCustomerId(request.CustomerId)
                                        .WithPaging(request.Skip, request.Take)
                                        .WithSorting(request.Sort)
                                        .Build();
            var searchResult = await _customerOrderSearchService.SearchCustomerOrdersAsync(searchCriteria);
            var aggregates = await _customerOrderAggregateRepository.GetAggregatesFromOrdersAsync(searchResult.Results);
            return new SearchOrderResponse { TotalCount = searchResult.TotalCount, Results = aggregates };
        }
    }
}
