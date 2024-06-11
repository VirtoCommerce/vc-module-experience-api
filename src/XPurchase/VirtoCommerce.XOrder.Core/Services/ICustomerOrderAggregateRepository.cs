using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Services
{
    public interface ICustomerOrderAggregateRepository
    {
        Task<CustomerOrderAggregate> GetOrderByIdAsync(string orderId);
        Task<CustomerOrderAggregate> CreateOrderFromCart(ShoppingCart cart);
        Task<CustomerOrderAggregate> GetAggregateFromOrderAsync(CustomerOrder order);
        Task<IList<CustomerOrderAggregate>> GetAggregatesFromOrdersAsync(IList<CustomerOrder> orders, string cultureName = null);
    }
}
