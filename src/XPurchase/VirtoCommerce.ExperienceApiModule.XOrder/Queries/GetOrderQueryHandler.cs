using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, CustomerOrderAggregate>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        public GetOrderQueryHandler(ICustomerOrderAggregateRepository customerOrderAggregateRepository)
        {
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
        }

        public Task<CustomerOrderAggregate> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.OrderId))
            {
                return _customerOrderAggregateRepository.GetOrderByIdAsync(request.OrderId);
            }
            else if (!string.IsNullOrEmpty(request.Number))
            {
                return _customerOrderAggregateRepository.GetOrderByNumberAsync(request.Number);
            }
            else
            {
                throw new ArgumentNullException($"{nameof(request.OrderId)} or {nameof(request.Number)}");
            }

        }
    }
}
