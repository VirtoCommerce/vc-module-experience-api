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
        private readonly ICustomerOrderSearchService _customerOrderSearchService;
        private readonly IMapper _mapper;
        private readonly ISearchPhraseParser _searchPhraseParser;

        public SearchOrderQueryHandler(ICustomerOrderSearchService customerOrderSearchService, IMapper mapper, ISearchPhraseParser searchPhraseParser)
        {
            _customerOrderSearchService = customerOrderSearchService;
            _mapper = mapper;
            _searchPhraseParser = searchPhraseParser;
        }

        public async Task<SearchOrderResponse> Handle(SearchOrderQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new CustomerOrderSearchCriteriaBuilder(_searchPhraseParser, _mapper)
                                        .ParseFilters(request.Filter)
                                        .WithPaging(request.Skip, request.Take)
                                        .AddSorting(request.Sort)
                                        .Build();
            var response = await _customerOrderSearchService.SearchCustomerOrdersAsync(searchCriteria);
            return new SearchOrderResponse { TotalCount = response.TotalCount, Results = response.Results };
        }
    }
}
