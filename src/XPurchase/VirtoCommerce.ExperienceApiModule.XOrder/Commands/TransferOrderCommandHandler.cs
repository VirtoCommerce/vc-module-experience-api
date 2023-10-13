using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class TransferOrderCommandHandler : IRequestHandler<TransferOrderCommand, CustomerOrderAggregate>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ICustomerOrderService _customerOrderService;

        public TransferOrderCommandHandler(ICustomerOrderService customerOrderService, ICustomerOrderAggregateRepository customerOrderAggregateRepository)
        {
            _customerOrderService = customerOrderService;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
        }

        public virtual async Task<CustomerOrderAggregate> Handle(TransferOrderCommand request, CancellationToken cancellationToken)
        {
            var orderAggregate = await _customerOrderAggregateRepository.GetOrderByIdAsync(request.CustomerOrderId);
            orderAggregate.TransferOrder(userId: request.ToUserId, userName: request.UserName);

            await _customerOrderService.SaveChangesAsync(new[] { orderAggregate.Order });
            return orderAggregate;
        }
    }
}
