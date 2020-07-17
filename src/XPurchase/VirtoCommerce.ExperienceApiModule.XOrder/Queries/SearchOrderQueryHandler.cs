using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrderQueryHandler : IQueryHandler<SearchOrderQuery, SearchOrderResponse>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly IMapper _mapper;
        private readonly ISearchPhraseParser _searchPhraseParser;

        public SearchOrderQueryHandler(IMapper mapper, ISearchPhraseParser searchPhraseParser, ICustomerOrderAggregateRepository customerOrderAggregateRepository)
        {
            _mapper = mapper;
            _searchPhraseParser = searchPhraseParser;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
        }

        public async Task<SearchOrderResponse> Handle(SearchOrderQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new CustomerOrderSearchCriteriaBuilder(_searchPhraseParser, _mapper)
                                        .ParseFilters(request.Filter)
                                        .WithPaging(request.Skip, request.Take)
                                        .AddSorting(request.Sort)
                                        .Build();
            var response = await _customerOrderAggregateRepository.SearchCustomerOrdersAsync(searchCriteria);
            return new SearchOrderResponse { TotalCount = response.Count, Results = response };
        }
    }
}
