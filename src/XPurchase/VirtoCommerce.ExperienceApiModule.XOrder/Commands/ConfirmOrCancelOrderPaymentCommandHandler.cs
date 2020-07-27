using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class ConfirmOrCancelOrderPaymentCommandHandler : IRequestHandler<CancelOrderPaymentCommand, bool>, IRequestHandler<ConfirmOrderPaymentCommand, bool>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ICustomerOrderService _customerOrderService;
        public ConfirmOrCancelOrderPaymentCommandHandler(ICustomerOrderService customerOrderService, ICustomerOrderAggregateRepository customerOrderAggregateRepository)
        {
            _customerOrderService = customerOrderService;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
        }

        public async Task<bool> Handle(CancelOrderPaymentCommand request, CancellationToken cancellationToken)
        {
            var orderAggregate = await _customerOrderAggregateRepository.GetOrderByIdAsync(request.Payment.OrderId);
            orderAggregate.CancelOrderPayment(request.Payment);

            await _customerOrderService.SaveChangesAsync(new[] { orderAggregate.Order });

            return true;
        }

        public async Task<bool> Handle(ConfirmOrderPaymentCommand request, CancellationToken cancellationToken)
        {
            var orderAggregate = await _customerOrderAggregateRepository.GetOrderByIdAsync(request.Payment.OrderId);
            orderAggregate.ConfirmOrderPayment(request.Payment);
            await _customerOrderService.SaveChangesAsync(new[] { orderAggregate.Order });
            return true;
        }
    }
}
