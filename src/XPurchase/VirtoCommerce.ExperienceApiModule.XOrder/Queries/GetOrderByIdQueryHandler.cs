using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, CustomerOrderAggregate>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        public GetOrderByIdQueryHandler(ICustomerOrderAggregateRepository customerOrderAggregateRepository)
        {
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
        }

        public Task<CustomerOrderAggregate> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return _customerOrderAggregateRepository.GetOrderByIdAsync(request.OrderId);
        }
    }
}
