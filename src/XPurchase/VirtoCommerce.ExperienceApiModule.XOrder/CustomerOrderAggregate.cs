using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderAggregate : Entity, IAggregateRoot
    {
        private readonly IDynamicPropertyUpdaterService _dynamicPropertyUpdaterService;

        public CustomerOrderAggregate(IDynamicPropertyUpdaterService dynamicPropertyUpdaterService)
        {
            _dynamicPropertyUpdaterService = dynamicPropertyUpdaterService;
        }

        public CustomerOrder Order { get; protected set; }
        public Currency Currency { get; protected set; }

        public CustomerOrderAggregate GrabCustomerOrder(CustomerOrder order, Currency currency)
        {
            Order = order;
            Currency = currency;

            return this;
        }

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

        public virtual async Task<CustomerOrderAggregate> UpdateOrderDynamicProperties(IList<DynamicPropertyValue> dynamicProperties)
        {
            await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(Order, dynamicProperties);

            return this;
        }

        public virtual async Task<CustomerOrderAggregate> UpdateItemDynamicProperties(string lineItemId, IList<DynamicPropertyValue> dynamicProperties)
        {
            var lineItem = Order.Items.FirstOrDefault(x => x.Id == lineItemId);
            if (lineItem != null)
            {
                await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(lineItem, dynamicProperties);
            }

            return this;
        }

        public virtual async Task<CustomerOrderAggregate> UpdateShipmentDynamicProperties(string shipmentId, IList<DynamicPropertyValue> dynamicProperties)
        {
            var shipment = Order.Shipments.FirstOrDefault(x => x.Id == shipmentId);
            if (shipment != null)
            {
                await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(shipment, dynamicProperties);
            }

            return this;
        }

        public virtual async Task<CustomerOrderAggregate> UpdatePaymentDynamicProperties(string paymentId, IList<DynamicPropertyValue> dynamicProperties)
        {
            var payment = Order.InPayments.FirstOrDefault(x => x.Id == paymentId);
            if (payment != null)
            {
                await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(payment, dynamicProperties);
            }

            return this;
        }
    }
}
