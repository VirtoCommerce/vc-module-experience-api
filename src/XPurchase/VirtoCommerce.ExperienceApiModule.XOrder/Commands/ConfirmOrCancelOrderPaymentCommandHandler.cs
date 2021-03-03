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
            var paymentCancelledSuccessful = orderAggregate.CancelOrderPayment(request.Payment);
            if (paymentCancelledSuccessful)
            {
                await _customerOrderService.SaveChangesAsync(new[] { orderAggregate.Order });
            }

            return paymentCancelledSuccessful;
        }

        public async Task<bool> Handle(ConfirmOrderPaymentCommand request, CancellationToken cancellationToken)
        {
            var orderAggregate = await _customerOrderAggregateRepository.GetOrderByIdAsync(request.Payment.OrderId);
            var paymentConfirmedSuccessful = orderAggregate.ConfirmOrderPayment(request.Payment);
            if (paymentConfirmedSuccessful)
            {
                await _customerOrderService.SaveChangesAsync(new[] { orderAggregate.Order });
            }

            return paymentConfirmedSuccessful;
        }
    }
}
