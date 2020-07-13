using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderAggregateRepository : ICustomerOrderAggregateRepository
    {
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ICustomerOrderSearchService _customerOrderSearchService;

        public CustomerOrderAggregateRepository(ICustomerOrderService customerOrderService, ICustomerOrderSearchService customerOrderSearchService)
        {
            _customerOrderService = customerOrderService;
            _customerOrderSearchService = customerOrderSearchService;
        }

        public async Task<CustomerOrderAggregate> GetOrderByIdAsync(string orderId)
        {
            var order = await _customerOrderService.GetByIdAsync(orderId);

            if (order != null)
            {
                return new CustomerOrderAggregate(order);
            }

            return null;
        }

        public async Task<CustomerOrderAggregate> GetOrderByNumberAsync(string number)
        {
            var order = (await _customerOrderSearchService.SearchCustomerOrdersAsync(new CustomerOrderSearchCriteria { Number = number })).Results.FirstOrDefault();

            if (order != null)
            {
                return new CustomerOrderAggregate(order);
            }

            return null;
        }
    }
}
