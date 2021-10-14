using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase
{
    public class ShipmentOptional
    {
        public Optional<string> Id { get; set; }
        public Optional<string> FulfillmentCenterId { get; set; }
        public Optional<decimal?> Length { get; set; }
        public Optional<decimal?> Height { get; set; }
        public Optional<string> MeasureUnit { get; set; }
        public Optional<string> ShipmentMethodOption { get; set; }
        public Optional<string> ShipmentMethodCode { get; set; }
        public Optional<decimal?> VolumetricWeight { get; set; }
        public Optional<decimal?> Weight { get; set; }
        public Optional<string> WeightUnit { get; set; }
        public Optional<decimal?> Width { get; set; }
        public Optional<string> Currency { get; set; }
        public Optional<decimal> Price { get; set; }
        public Optional<AddressOptional> DeliveryAddress { get; set; }
    }
}
