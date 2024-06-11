using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.XOrder.Core;
using VirtoCommerce.XOrder.Core.Queries;
using VirtoCommerce.XOrder.Core.Services;

namespace VirtoCommerce.XOrder.Data.Queries
{
    public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, CustomerOrderAggregate>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ICustomerOrderSearchService _customerOrderSearchService;

        public GetOrderQueryHandler(ICustomerOrderAggregateRepository customerOrderAggregateRepository, ICustomerOrderSearchService customerOrderSearchService)
        {
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
            _customerOrderSearchService = customerOrderSearchService;
        }

        public virtual async Task<CustomerOrderAggregate> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            CustomerOrderAggregate result;
            if (!string.IsNullOrEmpty(request.OrderId))
            {
                result = await _customerOrderAggregateRepository.GetOrderByIdAsync(request.OrderId);
            }
            else if (!string.IsNullOrEmpty(request.Number))
            {
                var response = await _customerOrderSearchService.SearchAsync(new CustomerOrderSearchCriteria { Number = request.Number });
                result = await _customerOrderAggregateRepository.GetAggregateFromOrderAsync(response.Results.FirstOrDefault());
            }
            else
            {
                throw new ArgumentNullException($"{nameof(request.OrderId)} or {nameof(request.Number)}");
            }

            return result;
        }
    }
}
