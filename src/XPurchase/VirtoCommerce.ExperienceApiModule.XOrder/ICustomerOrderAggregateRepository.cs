using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public interface ICustomerOrderAggregateRepository
    {
        Task<CustomerOrderAggregate> GetOrderByIdAsync(string orderId);
        Task<CustomerOrderAggregate> GetOrderByNumberAsync(string number);
    }
}
