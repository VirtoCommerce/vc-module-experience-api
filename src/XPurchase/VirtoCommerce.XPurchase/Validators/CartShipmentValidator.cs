using System.Linq;
using FluentValidation;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartShipmentValidator : AbstractValidator<ShipmentValidationContext>
    {
        public CartShipmentValidator()
        {
            //To support the use case for partial shipment update when user sets the address first.
            //RuleFor(x => x.ShipmentMethodCode).NotNull().NotEmpty();
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((shipmentContext, context) =>
                {
                    var availShippingRates = shipmentContext.AvailShippingRates;
                    var shipment = shipmentContext.Shipment;
                    if (availShippingRates != null && !string.IsNullOrEmpty(shipment.ShipmentMethodCode))
                    {
                        var shipmentShippingMethod = availShippingRates.FirstOrDefault(sm => shipment.ShipmentMethodCode.EqualsInvariant(sm.ShippingMethod.Code) && shipment.ShipmentMethodOption.EqualsInvariant(sm.OptionName));
                        if (shipmentShippingMethod == null)
                        {
                            context.AddFailure(CartErrorDescriber.ShipmentMethodUnavailable(shipment, shipment.ShipmentMethodCode, shipment.ShipmentMethodOption));
                        }
                        else if (shipmentShippingMethod.Rate != shipment.Price)
                        {
                            context.AddFailure(CartErrorDescriber.ShipmentMethodPriceChanged(shipment, shipment.Price, shipment.PriceWithTax, shipmentShippingMethod.Rate, shipmentShippingMethod.RateWithTax));
                        }
                    }
                });
            });
        }
    }
}
