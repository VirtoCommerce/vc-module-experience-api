using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SearchPaymentMethodsQueryHandler : IQueryHandler<SearchPaymentMethodsQuery, PaymentMethodsSearchResult>
    {
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;

        public SearchPaymentMethodsQueryHandler(IPaymentMethodsSearchService paymentMethodsSearchService)
        {
            _paymentMethodsSearchService = paymentMethodsSearchService;
        }

        public virtual async Task<PaymentMethodsSearchResult> Handle(SearchPaymentMethodsQuery request, CancellationToken cancellationToken)
        {
            var criteria = AbstractTypeFactory<PaymentMethodsSearchCriteria>.TryCreateInstance();

            criteria.StoreId = request.StoreId;
            criteria.IsActive = true;
            criteria.Skip = request.Skip;
            criteria.Take = request.Take;

            var searchResult = await _paymentMethodsSearchService.SearchPaymentMethodsAsync(criteria);
            return searchResult;
        }
    }
}
