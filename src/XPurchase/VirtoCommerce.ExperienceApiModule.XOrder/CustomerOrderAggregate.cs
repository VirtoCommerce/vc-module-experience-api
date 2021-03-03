using System;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
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

        public bool CancelOrderPayment(PaymentIn payment)
        {
            var paymentOrder = Order.InPayments.FirstOrDefault(x => x.Number.EqualsInvariant(payment.Number));
            if (paymentOrder != null)
            {
                paymentOrder.IsCancelled = true;
                paymentOrder.CancelReason = payment.CancelReason ?? paymentOrder.CancelReason;
                paymentOrder.CancelledDate = payment.CancelledDate ?? DateTime.Now;
                paymentOrder.Status = PaymentStatus.Cancelled.ToString();
                paymentOrder.PaymentStatus = PaymentStatus.Cancelled;
                return true;
            }

            return false;
        }

        public bool ConfirmOrderPayment(PaymentIn payment)
        {
            var paymentOrder = Order.InPayments.FirstOrDefault(x => x.Number.EqualsInvariant(payment.Number));
            if (paymentOrder != null)
            {
                paymentOrder.BillingAddress = payment.BillingAddress;
                paymentOrder.IsApproved = true;
                paymentOrder.IsCancelled = false;
                paymentOrder.PaymentStatus = PaymentStatus.Paid;
                paymentOrder.Status = PaymentStatus.Paid.ToString();
                return true;
            }

            return false;
        }
    }
}
