using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Model;
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

        public Task<CustomerOrder> GetOrderByIdAsync(string orderId)
        {
            return _customerOrderService.GetByIdAsync(orderId);
        }

        public async Task<CustomerOrder> GetOrderByNumberAsync(string number)
        {
            var order = (await _customerOrderSearchService.SearchCustomerOrdersAsync(new CustomerOrderSearchCriteria { Number = number })).Results.FirstOrDefault();

            return order;
        }
    }
}
