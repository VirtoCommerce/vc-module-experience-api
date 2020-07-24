using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class ConfirmOrCancelOrderPaymentCommandHandler : IRequestHandler<CancelOrderPaymentCommand, bool>, IRequestHandler<ConfirmOrderPaymentCommand, bool>
    {
        private readonly ICustomerOrderService _customerOrderService;
        public ConfirmOrCancelOrderPaymentCommandHandler(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }
        public async Task<bool> Handle(CancelOrderPaymentCommand request, CancellationToken cancellationToken)
        {
            var order = await _customerOrderService.GetByIdAsync(request.Payment.OrderId);
            var payment = order.InPayments.FirstOrDefault(x => x.Number.EqualsInvariant(request.Payment.Number));
            if (payment != null)
            {
                payment.IsCancelled = request.Payment.IsCancelled;
                payment.CancelReason = request.Payment.CancelReason;
                payment.CancelledDate = request.Payment.CancelledDate;
                payment.Status = request.Payment.Status;

                await _customerOrderService.SaveChangesAsync(new[] { order });
                return true;
            }

            return false;
        }

        public async Task<bool> Handle(ConfirmOrderPaymentCommand request, CancellationToken cancellationToken)
        {
            var order = await _customerOrderService.GetByIdAsync(request.Payment.OrderId);
            var paymentOrder = order.InPayments.FirstOrDefault(x => x.Number.EqualsInvariant(request.Payment.Number));
            if (paymentOrder == null)
            {
                paymentOrder = request.Payment;
                order.InPayments.Add(paymentOrder);
            }
            else
            {
                paymentOrder.BillingAddress = request.Payment.BillingAddress;
            }

            await _customerOrderService.SaveChangesAsync(new[] { order });
            return true;
        }
    }
}
