using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Models
{
    public sealed class ExpOrderPayment
    {
        public Optional<string> Id { get; set; }
        public Optional<string> OuterId { get; set; }
        public Optional<string> PaymentGatewayCode { get; set; }
        public Optional<string> Currency { get; set; }
        public Optional<decimal> Price { get; set; }
        public Optional<decimal> Amount { get; set; }

        public Optional<ExpOrderAddress> BillingAddress { get; set; }

        public IList<DynamicPropertyValue> DynamicProperties { get; set; }

        public PaymentIn MapTo(PaymentIn payment)
        {
            payment ??= AbstractTypeFactory<PaymentIn>.TryCreateInstance();

            if (Id?.IsSpecified == true)
            {
                payment.Id = Id.Value;
            }

            if (OuterId?.IsSpecified == true)
            {
                payment.OuterId = OuterId.Value;
            }

            if (PaymentGatewayCode?.IsSpecified == true)
            {
                payment.GatewayCode = PaymentGatewayCode.Value;
            }

            if (Currency?.IsSpecified == true)
            {
                payment.Currency = Currency.Value;
            }

            if (Price?.IsSpecified == true)
            {
                payment.Price = Price.Value;
            }

            if (Amount?.IsSpecified == true)
            {
                payment.Sum = Amount.Value;
            }

            if (BillingAddress?.IsSpecified == true)
            {
                payment.BillingAddress = BillingAddress.Value?.MapTo(payment.BillingAddress) ?? null;
            }

            return payment;
        }
    }
}
