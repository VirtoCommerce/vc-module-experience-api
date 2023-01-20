using System.Collections.Generic;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase
{
    public sealed class ExpCartPayment
    {
        public Optional<string> Id { get; set; }
        public Optional<string> OuterId { get; set; }
        public Optional<string> PaymentGatewayCode { get; set; }
        public Optional<string> Currency { get; set; }
        public Optional<decimal> Price { get; set; }
        public Optional<decimal> Amount { get; set; }
        public Optional<string> VendorId { get; set; }

        public Optional<ExpCartAddress> BillingAddress { get; set; }

        public IList<DynamicPropertyValue> DynamicProperties { get; set; }

        public Payment MapTo(Payment payment)
        {
            if (payment == null)
            {
                payment = AbstractTypeFactory<Payment>.TryCreateInstance();
            }

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
                payment.PaymentGatewayCode = PaymentGatewayCode.Value;
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
                payment.Amount = Amount.Value;
            }

            if (VendorId?.IsSpecified == true)
            {
                payment.VendorId = VendorId.Value;
            }

            if (BillingAddress?.IsSpecified == true)
            {
                payment.BillingAddress = BillingAddress.Value?.MapTo(payment.BillingAddress) ?? null;
            }

            return payment;
        }
    }
}
