using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.OrdersModule.Core.Model.Search;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchPaymentsQuery : IQuery<PaymentSearchResult>
    {
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string CultureName { get; set; }
        public string CustomerId { get; set; }
    }
}
