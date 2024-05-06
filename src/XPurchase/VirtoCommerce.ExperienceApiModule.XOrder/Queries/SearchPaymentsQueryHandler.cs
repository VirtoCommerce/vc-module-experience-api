using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchPaymentsQueryHandler : IQueryHandler<SearchPaymentsQuery, PaymentSearchResult>
    {
        private readonly IMapper _mapper;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IPaymentSearchService _paymentsSearchService;

        public SearchPaymentsQueryHandler(IMapper mapper,
            ISearchPhraseParser searchPhraseParser,
            IPaymentSearchService paymentsSearchService)
        {
            _mapper = mapper;
            _searchPhraseParser = searchPhraseParser;
            _paymentsSearchService = paymentsSearchService;
        }

        public virtual async Task<PaymentSearchResult> Handle(SearchPaymentsQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new PaymentsSearchCriteriaBuilder(_searchPhraseParser, _mapper)
                                        .ParseFilters(request.Filter)
                                        .WithCustomerId(request.CustomerId)
                                        .WithPaging(request.Skip, request.Take)
                                        .WithSorting(request.Sort)
                                        .Build();
            var searchResult = await _paymentsSearchService.SearchAsync(searchCriteria);
            return searchResult;
        }
    }
}
