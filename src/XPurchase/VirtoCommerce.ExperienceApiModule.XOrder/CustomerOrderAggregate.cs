using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XOrder.Validators;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderAggregate : Entity, IAggregateRoot, ICloneable
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

        public ProcessPaymentRequestResult ProcessOrderPayment(ProcessPaymentRequest request)
        {
            var result = new ProcessPaymentRequestResult();

            //Do not allow to mutate order internal state in a payment gateway code
            request.Order = Order.Clone() as CustomerOrder;
            var inPayment = Order.InPayments.FirstOrDefault(x => x.Id == request.PaymentId);
            if (inPayment != null)
            {
                // Do not allow to mutate payment internal state in a payment gateway code
                request.Payment = inPayment.Clone() as PaymentIn;
            }

            var validationResult = AbstractTypeFactory<ProcessPaymentRequestVaidator>.TryCreateInstance().Validate(request);
            if (!validationResult.IsValid)
            {
                return new ProcessPaymentRequestResult
                {
                    IsSuccess = false,
                    ErrorMessage = string.Join(' ', validationResult.Errors)
                };
            }

            if (inPayment != null)
            {
                //This is definetelly bad that we execute external business logic here, it must be done via domain events or in the event handler
                //inside this aggregate we should do only related to order entities changes and shoud avoid of execution of external logic
                result = inPayment.PaymentMethod.ProcessPayment(request);
                if (result.OuterId != null)
                {
                    inPayment.OuterId = result.OuterId;
                }
                //Update internal state with data from results
                inPayment.Status = result.NewPaymentStatus.ToString();
                inPayment.Transactions = ((PaymentIn)request.Payment).Transactions;
            }

            return result;
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

        public object Clone()
        {
            var result = MemberwiseClone() as CustomerOrderAggregate;
            result.Order = Order?.Clone() as CustomerOrder;
            result.Currency = Currency?.Clone() as Currency;
            return result;
        }
    }
}
