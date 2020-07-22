using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public interface ICustomerOrderAggregateRepository
    {
        Task<CustomerOrderAggregate> GetOrderByIdAsync(string orderId);
        Task<CustomerOrderAggregate> CreateOrderFromCart(ShoppingCart cart);
        Task<CustomerOrderAggregate> GetAggregateFromOrderAsync(CustomerOrder orders);
        Task<IList<CustomerOrderAggregate>> GetAggregatesFromOrdersAsync(IList<CustomerOrder> orders, string cultureName = null);
    }
}
