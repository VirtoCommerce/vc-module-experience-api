using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.XOrder.Core;
using VirtoCommerce.XOrder.Core.Commands;
using VirtoCommerce.XOrder.Core.Services;

namespace VirtoCommerce.XOrder.Data.Commands
{
    public class UpdateOrderShipmentDynamicPropertiesCommandHandler : IRequestHandler<UpdateOrderShipmentDynamicPropertiesCommand, CustomerOrderAggregate>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ICustomerOrderService _customerOrderService;

        public UpdateOrderShipmentDynamicPropertiesCommandHandler(ICustomerOrderService customerOrderService, ICustomerOrderAggregateRepository customerOrderAggregateRepository)
        {
            _customerOrderService = customerOrderService;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
        }

        public virtual async Task<CustomerOrderAggregate> Handle(UpdateOrderShipmentDynamicPropertiesCommand request, CancellationToken cancellationToken)
        {
            var orderAggregate = await _customerOrderAggregateRepository.GetOrderByIdAsync(request.OrderId);

            await orderAggregate.UpdateShipmentDynamicProperties(request.ShipmentId, request.DynamicProperties);

            await _customerOrderService.SaveChangesAsync(new[] { orderAggregate.Order });

            return orderAggregate;
        }
    }
}
