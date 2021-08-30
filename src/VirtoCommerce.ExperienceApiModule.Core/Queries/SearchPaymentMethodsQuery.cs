using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.PaymentModule.Core.Model.Search;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SearchPaymentMethodsQuery : IQuery<PaymentMethodsSearchResult>
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public string StoreId { get; set; }
    }
}
