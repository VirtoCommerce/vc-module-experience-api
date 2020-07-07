using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class TaxExtensions
    {
        public static void ApplyTaxRates(this PaymentMethod paymentMethod, IEnumerable<TaxRate> taxRates)
        {
            paymentMethod.TaxPercentRate = 0m;
            var paymentTaxRate = taxRates.FirstOrDefault(x => x.Line.Id != null && x.Line.Id.EqualsInvariant(paymentMethod.Code ?? ""));

            if (paymentTaxRate == null)
            {
                return;
            }

            if (paymentTaxRate.PercentRate > 0)
            {
                paymentMethod.TaxPercentRate = paymentTaxRate.PercentRate;
            }
            else
            {
                var amount = paymentMethod.Total > 0 ? paymentMethod.Total : paymentMethod.Price;
                if (amount > 0)
                {
                    paymentMethod.TaxPercentRate = Math.Round(paymentTaxRate.Rate / amount, 4, MidpointRounding.AwayFromZero);
                }
            }

            paymentMethod.TaxDetails = paymentTaxRate.TaxDetails;
        }

        [Obsolete("Remove if ApplyTaxRates(this ShippingRate shippingRate, IEnumerable<TaxRate> taxRates) will be written")]
        public static void ApplyTaxRates(this ShipmentMethod shipmentMethod, IEnumerable<TaxRate> taxRates)
        {
            shipmentMethod.TaxPercentRate = 0m;
            var taxLineId = string.Join("&", shipmentMethod.ShipmentMethodCode, shipmentMethod.OptionName);
            var taxRate = taxRates.FirstOrDefault(x => x.Line.Id == taxLineId);

            if (taxRate != null && taxRate.Rate > 0)
            {
                if (taxRate.PercentRate > 0)
                {
                    shipmentMethod.TaxPercentRate = taxRate.PercentRate;
                }
                else
                {
                    var amount = shipmentMethod.Total > 0 ? shipmentMethod.Total : shipmentMethod.Price;
                    if (amount > 0)
                    {
                        shipmentMethod.TaxPercentRate = Math.Round(taxRate.Rate / amount, 4, MidpointRounding.AwayFromZero);
                    }
                }

                // TODO: No TaxDetails in shipmentMethod
                //shipmentMethod.TaxDetails = taxRate.TaxDetails;
            }
        }

        public static void ApplyTaxRates(this ShippingRate shippingRate, IEnumerable<TaxRate> taxRates)
        {
            // TODO: Write and use new model for resolve taxable logic for ShippingRate/ShippingMethod
        }

        public static void ApplyTaxRates(this LineItem lineItem, IEnumerable<TaxRate> taxRates)
        {
            lineItem.TaxPercentRate = 0m;
            var lineItemTaxRate = taxRates.FirstOrDefault(x => x.Line.Id != null && x.Line.Id.EqualsInvariant(lineItem.Id ?? ""))
                ?? taxRates.FirstOrDefault(x => x.Line.Code != null && x.Line.Code.EqualsInvariant(lineItem.Sku ?? ""));

            if (lineItemTaxRate == null)
            {
                return;
            }

            if (lineItemTaxRate.PercentRate > 0)
            {
                lineItem.TaxPercentRate = lineItemTaxRate.PercentRate;
            }
            else
            {
                var amount = lineItem.ExtendedPrice > 0 ? lineItem.ExtendedPrice : lineItem.SalePrice;
                if (amount > 0)
                {
                    lineItem.TaxPercentRate = Math.Round(lineItemTaxRate.Rate / amount, 4, MidpointRounding.AwayFromZero);
                }
            }

            lineItem.TaxDetails = lineItemTaxRate.TaxDetails;
        }

        public static void ApplyTaxRates(this Payment payment, IEnumerable<TaxRate> taxRates)
        {
            payment.TaxPercentRate = 0m;
            var paymentTaxRate = taxRates.FirstOrDefault(x => x.Line.Id != null && x.Line.Id.EqualsInvariant(payment.Id ?? ""))
                ?? taxRates.FirstOrDefault(x => x.Line.Code != null && x.Line.Code.EqualsInvariant(payment.PaymentGatewayCode));

            if (paymentTaxRate == null)
            {
                return;
            }

            if (paymentTaxRate.PercentRate > 0)
            {
                payment.TaxPercentRate = paymentTaxRate.PercentRate;
            }
            else
            {
                var amount = payment.Total > 0 ? payment.Total : payment.Price;
                if (amount > 0)
                {
                    payment.TaxPercentRate = Math.Round(paymentTaxRate.Rate / amount, 4, MidpointRounding.AwayFromZero);
                }
            }

            payment.TaxDetails = paymentTaxRate.TaxDetails;
        }

        public static void ApplyTaxRates(this Shipment shipment, IEnumerable<TaxRate> taxRates)
        {
            shipment.TaxPercentRate = 0m;
            var shipmentTaxRate = taxRates.FirstOrDefault(x => x.Line.Id != null && x.Line.Id.EqualsInvariant(shipment.Id ?? ""))
                ?? taxRates.FirstOrDefault(x => x.Line.Code != null && x.Line.Code.EqualsInvariant(shipment.ShipmentMethodCode));

            if (shipmentTaxRate == null || shipmentTaxRate.Rate <= 0)
            {
                return;
            }

            if (shipmentTaxRate.PercentRate > 0)
            {
                shipment.TaxPercentRate = shipmentTaxRate.PercentRate;
            }
            else
            {
                var amount = shipment.Total > 0 ? shipment.Total : shipment.Price;
                if (amount > 0)
                {
                    shipment.TaxPercentRate = Math.Round(shipmentTaxRate.Rate / amount, 4, MidpointRounding.AwayFromZero);
                }
            }

            shipment.TaxDetails = shipmentTaxRate.TaxDetails;
        }

        public static void ApplyTaxRates(this ShoppingCart shoppingCart, IEnumerable<TaxRate> taxRates)
        {
            shoppingCart.TaxPercentRate = 0m;
            foreach (var lineItem in shoppingCart.Items ?? Enumerable.Empty<LineItem>())
            {
                //Get percent rate from line item
                if (shoppingCart.TaxPercentRate == 0)
                {
                    shoppingCart.TaxPercentRate = lineItem.TaxPercentRate;
                }
                lineItem.ApplyTaxRates(taxRates);
            }
            foreach (var shipment in shoppingCart.Shipments ?? Enumerable.Empty<Shipment>())
            {
                shipment.ApplyTaxRates(taxRates);
            }
            foreach (var payment in shoppingCart.Payments ?? Enumerable.Empty<Payment>())
            {
                payment.ApplyTaxRates(taxRates);
            }
        }
    }
}
