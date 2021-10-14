using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase
{
    public class PaymentOptional
    {
        public Optional<string> Id { get; set; }
        public Optional<string> OuterId { get; set; }
        public Optional<string> PaymentGatewayCode { get; set; }
        public Optional<string> Currency { get; set; }
        public Optional<decimal> Price { get; set; }
        public Optional<decimal> Amount { get; set; }

        public Optional<AddressOptional> BillingAddress { get; set; }
    }
}
