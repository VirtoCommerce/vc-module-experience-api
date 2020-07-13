using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class GetOrderByNumberQueryHandler : IQueryHandler<GetOrderByNumberQuery, CustomerOrderAggregate>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;

        public GetOrderByNumberQueryHandler(ICustomerOrderAggregateRepository customerOrderAggregateRepository)
        {
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
        }

        public Task<CustomerOrderAggregate> Handle(GetOrderByNumberQuery request, CancellationToken cancellationToken)
        {
            return _customerOrderAggregateRepository.GetOrderByNumberAsync(request.Number);
        }
    }
}
