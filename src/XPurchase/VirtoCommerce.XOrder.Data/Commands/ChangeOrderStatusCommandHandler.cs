using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.XOrder.Core.Commands;
using VirtoCommerce.XOrder.Core.Services;

namespace VirtoCommerce.XOrder.Data.Commands
{
    public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, bool>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ICustomerOrderService _customerOrderService;
        public ChangeOrderStatusCommandHandler(ICustomerOrderService customerOrderService, ICustomerOrderAggregateRepository customerOrderAggregateRepository)
        {
            _customerOrderService = customerOrderService;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
        }
        public virtual async Task<bool> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderAggregate = await _customerOrderAggregateRepository.GetOrderByIdAsync(request.OrderId);
            orderAggregate.ChangeOrderStatus(request.Status);
            await _customerOrderService.SaveChangesAsync(new[] { orderAggregate.Order });
            return true;
        }
    }
}
