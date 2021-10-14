using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase
{
    public sealed class ExpCartShipment
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
        public Optional<ExpCartAddress> DeliveryAddress { get; set; }

        public Shipment MapTo(Shipment shipment)
        {
            if (shipment == null)
            {
                shipment = AbstractTypeFactory<Shipment>.TryCreateInstance();
            }

            if (Id?.IsSpecified == true)
            {
                shipment.Id = Id.Value;
            }

            if (FulfillmentCenterId?.IsSpecified == true)
            {
                shipment.FulfillmentCenterId = FulfillmentCenterId.Value;
            }

            if (Length?.IsSpecified == true)
            {
                shipment.Length = Length.Value;
            }

            if (Height?.IsSpecified == true)
            {
                shipment.Height = Height.Value;
            }

            if (MeasureUnit?.IsSpecified == true)
            {
                shipment.MeasureUnit = MeasureUnit.Value;
            }

            if (ShipmentMethodOption?.IsSpecified == true)
            {
                shipment.ShipmentMethodOption = ShipmentMethodOption.Value;
            }

            if (ShipmentMethodCode?.IsSpecified == true)
            {
                shipment.ShipmentMethodCode = ShipmentMethodCode.Value;
            }

            if (VolumetricWeight?.IsSpecified == true)
            {
                shipment.VolumetricWeight = VolumetricWeight.Value;
            }

            if (Weight?.IsSpecified == true)
            {
                shipment.Weight = Weight.Value;
            }

            if (WeightUnit?.IsSpecified == true)
            {
                shipment.WeightUnit = WeightUnit.Value;
            }

            if (Width?.IsSpecified == true)
            {
                shipment.Width = Width.Value;
            }

            if (Currency?.IsSpecified == true)
            {
                shipment.Currency = Currency.Value;
            }

            if (Price?.IsSpecified == true)
            {
                shipment.Price = Price.Value;
            }

            if (DeliveryAddress?.IsSpecified == true)
            {
                shipment.DeliveryAddress = DeliveryAddress.Value?.MapTo(shipment.DeliveryAddress) ?? null;
            }

            return shipment;
        }
    }
}
