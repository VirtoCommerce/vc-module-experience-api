using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.XOrder.Core;
using VirtoCommerce.XOrder.Core.Commands;
using VirtoCommerce.XOrder.Core.Services;

namespace VirtoCommerce.XOrder.Data.Commands
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

            if (orderAggregate.Order.IsAnonymous)
            {
                orderAggregate.Order.IsAnonymous = false;
                orderAggregate.Order.CustomerId = request.ToUserId;
                orderAggregate.Order.CustomerName = request.UserName;

                await _customerOrderService.SaveChangesAsync(new[] { orderAggregate.Order });
            }

            return orderAggregate;
        }
    }
}
