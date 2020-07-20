using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Model.Search;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public interface ICustomerOrderAggregateRepository
    {
        Task<CustomerOrderAggregate> GetOrderByIdAsync(string orderId);
        Task<CustomerOrderAggregate> GetOrderByNumberAsync(string number);
        Task<IList<CustomerOrderAggregate>> SearchCustomerOrdersAsync(CustomerOrderSearchCriteria searchCriteria, string language = null);
    }
}
