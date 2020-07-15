using System.Collections.Generic;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrderResponse
    {
        public int TotalCount { get; set; }
        public IList<CustomerOrder> Results { get; set; }
        public IList<CustomerOrder> CustomerOrders => Results;
    }
}
