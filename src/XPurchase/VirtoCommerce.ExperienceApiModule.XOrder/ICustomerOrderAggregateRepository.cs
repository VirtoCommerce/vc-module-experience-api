using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public interface ICustomerOrderAggregateRepository
    {
        Task<CustomerOrder> GetOrderByIdAsync(string orderId);
        Task<CustomerOrder> GetOrderByNumberAsync(string number);
    }
}
