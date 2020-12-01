using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartShipmentValidator : AbstractValidator<Shipment>
    {
        public CartShipmentValidator(IEnumerable<ShippingRate> availShippingRates)
        {
            RuleFor(x => x.ShipmentMethodCode).NotNull().NotEmpty();
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((shipment, context) =>
                {
                    if (availShippingRates != null)
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
