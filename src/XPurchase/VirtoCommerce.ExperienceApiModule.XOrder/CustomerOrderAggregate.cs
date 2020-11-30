using System.Linq;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderAggregate : Entity, IAggregateRoot
    {

        public CustomerOrderAggregate(CustomerOrder order, Currency currency)
        {
            Order = order;
            Currency = currency;
        }

        public CustomerOrder Order { get; protected set; }
        public Currency Currency { get; protected set; }

        public void ChangeOrderStatus(string status)
        {
            Order.Status = status;
        }

        public void CancelOrderPayment(PaymentIn payment)
        {
            var paymentOrder = Order.InPayments.FirstOrDefault(x => x.Number.EqualsInvariant(payment.Number));
            if (paymentOrder != null)
            {
                paymentOrder.IsCancelled = payment.IsCancelled;
                paymentOrder.CancelReason = payment.CancelReason;
                paymentOrder.CancelledDate = payment.CancelledDate;
                paymentOrder.PaymentStatus = payment.PaymentStatus;
                paymentOrder.Status = payment.Status;
            }
        }

        public void ConfirmOrderPayment(PaymentIn payment)
        {
            var paymentOrder = Order.InPayments.FirstOrDefault(x => x.Number.EqualsInvariant(payment.Number));
            if (paymentOrder == null)
            {
                paymentOrder = payment;
                Order.InPayments.Add(paymentOrder);
            }
            else
            {
                paymentOrder.BillingAddress = payment.BillingAddress;
            }
        }
    }
}
