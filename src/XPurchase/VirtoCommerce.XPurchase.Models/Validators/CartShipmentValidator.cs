using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.XPurchase.Models.Cart;
using VirtoCommerce.XPurchase.Models.Cart.ValidationErrors;

namespace VirtoCommerce.XPurchase.Models.Validators
{
    public class CartShipmentValidator : AbstractValidator<Shipment>
    {
        public CartShipmentValidator(IEnumerable<ShippingMethod> availableShippingMethods)
        {
            RuleSet("strict", () =>
            {
                RuleFor(x => x).CustomAsync(async (shipment, context, cancellationToken) =>
                {
                    shipment.ValidationErrors.Clear();

                    var shipmentShippingMethod = availableShippingMethods.FirstOrDefault(sm => shipment.HasSameMethod(sm));
                    if (shipmentShippingMethod == null)
                    {
                        var unavailableError = new UnavailableError();
                        shipment.ValidationErrors.Add(unavailableError);
                        context.AddFailure(new ValidationFailure(nameof(shipment.ShipmentMethodCode), "The shipment method is no longer available"));
                    }
                    else if (shipmentShippingMethod.Price != shipment.Price)
                    {
                        var priceChangedError = new PriceError(shipment.Price, shipment.PriceWithTax, shipmentShippingMethod.Price, shipmentShippingMethod.PriceWithTax);
                        shipment.ValidationErrors.Add(priceChangedError);
                        context.AddFailure(new ValidationFailure(nameof(shipment.Price), "The shipment method price is changed"));
                    }
                });
            });
        }
    }
}
