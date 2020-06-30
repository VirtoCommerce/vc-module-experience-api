using System.Linq;
using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartShipmentValidator : AbstractValidator<Shipment>
    {
        public CartShipmentValidator(CartAggregate cartAggr)
        {
            RuleSet("strict", () =>
            {
                RuleFor(x => x).CustomAsync(async (shipment, context, cancellationToken) =>
                {
                    var shipmentShippingMethod = (await cartAggr.GetAvailableShippingRatesAsync()).FirstOrDefault(sm => shipment.ShipmentMethodCode.EqualsInvariant(sm.ShippingMethod.Code) && shipment.ShipmentMethodOption.EqualsInvariant(sm.OptionName));
                    if (shipmentShippingMethod == null)
                    {
                        context.AddFailure(CartErrorDescriber.ShipmentMethodUnavailable(shipment, shipment.ShipmentMethodCode));
                    }
                    else if (shipmentShippingMethod.Rate != shipment.Price)
                    {
                        context.AddFailure(CartErrorDescriber.ShipmentMethodPriceChanged(shipment, shipment.Price, shipment.PriceWithTax, shipmentShippingMethod.Rate, shipmentShippingMethod.RateWithTax));
                    }
                });
            });
        }
    }
}
