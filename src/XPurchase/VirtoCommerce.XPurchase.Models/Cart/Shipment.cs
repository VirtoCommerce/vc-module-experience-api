using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.XPurchase.Models.Cart.ValidationErrors;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.Extensions;
using VirtoCommerce.XPurchase.Models.Marketing;
using VirtoCommerce.XPurchase.Models.Tax;

namespace VirtoCommerce.XPurchase.Models.Cart
{
    public class Shipment : CloneableEntity, IDiscountable, IValidatable, ITaxable
    {
        public Shipment()
        {
            Discounts = new List<Discount>();
            Items = new List<CartShipmentItem>();
            TaxDetails = new List<TaxDetail>();
            ValidationErrors = new List<ValidationError>();
            IsValid = true;
        }

        public Shipment(Currency currency)
            : this()
        {
            Currency = currency;

            Price = new Money(currency);
            PriceWithTax = new Money(currency);
            DiscountAmount = new Money(currency);
            DiscountAmountWithTax = new Money(currency);
            Total = new Money(currency);
            TotalWithTax = new Money(currency);
            TaxTotal = new Money(currency);
        }

        /// <summary>
        /// Gets or sets the value of shipping method code.
        /// </summary>
        public string ShipmentMethodCode { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping method option.
        /// </summary>
        public string ShipmentMethodOption { get; set; }

        /// <summary>
        /// Gets or sets the value of fulfillment center id.
        /// </summary>
        public string FulfillmentCenterId { get; set; }

        /// <summary>
        /// Gets or sets the delivery address.
        /// </summary>
        /// <value>
        /// Address object.
        /// </value>
        public Address DeliveryAddress { get; set; }

        /// <summary>
        /// Gets or sets the value of volumetric weight.
        /// </summary>
        public decimal? VolumetricWeight { get; set; }

        /// <summary>
        /// Gets or sets the value of weight unit.
        /// </summary>
        public string WeightUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of weight.
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// Gets or sets the value of measurement units.
        /// </summary>
        public string MeasureUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of height.
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Gets or sets the value of length.
        /// </summary>
        public double? Length { get; set; }

        /// <summary>
        /// Gets or sets the value of width.
        /// </summary>
        public double? Width { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping price.
        /// </summary>
        public Money Price { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping price including tax
        /// Price * TaxPercentRate.
        /// </summary>
        public Money PriceWithTax { get; set; }

        /// <summary>
        /// Gets or sets the value of total shipping price without taxes
        /// Price + Fee - DiscountAmount;.
        /// </summary>
        public Money Total { get; set; }

        /// <summary>
        /// Gets or sets the value of total shipping price including taxes
        /// Total * TaxPercentRate.
        /// </summary>
        public Money TotalWithTax { get; set; }

        /// <summary>
        /// Gets or sets the value of total shipping discount amount.
        /// </summary>
        public Money DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the value of discount including taxes
        /// DiscountAmount * TaxPercentRate.
        /// </summary>
        public Money DiscountAmountWithTax { get; set; }

        /// <summary>
        /// Gets or sets the collection of shipping items.
        /// </summary>
        /// <value>
        /// Collection of CartShipmentItem objects.
        /// </value>
        public IList<CartShipmentItem> Items { get; set; }


        /// <summary>
        /// Gets or sets the value of total shipping tax amount.
        /// </summary>
        public Money TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping tax type.
        /// </summary>
        public string TaxType { get; set; }

        /// <summary>
        /// Gets or sets the collection of line item tax details lines.
        /// </summary>
        /// <value>
        /// Collection of TaxDetail objects.
        /// </value>
        public IList<TaxDetail> TaxDetails { get; set; }

        public bool IsValid { get; set; }

        public IList<ValidationError> ValidationErrors { get; set; }

        public IList<Discount> Discounts { get; private set; }

        public Currency Currency { get; set; }

        public void ApplyTaxRates(IEnumerable<TaxRate> taxRates)
        {
            TaxPercentRate = 0m;
            var taxRatesList = taxRates.ToList();
            var shipmentTaxRate = taxRatesList.FirstOrDefault(x => x.Line.Id != null && x.Line.Id.EqualsInvariant(Id ?? string.Empty));
            if (shipmentTaxRate == null)
            {
                shipmentTaxRate = taxRatesList.FirstOrDefault(x => x.Line.Code.EqualsInvariant(ShipmentMethodCode)
                                && x.Line.Name.EqualsInvariant(ShipmentMethodOption));
            }

            if (shipmentTaxRate != null && shipmentTaxRate.Rate.Amount > 0)
            {
                if (shipmentTaxRate.PercentRate > 0)
                {
                    TaxPercentRate = shipmentTaxRate.PercentRate;
                }
                else
                {
                    var amount = Total.Amount > 0 ? Total.Amount : Price.Amount;
                    if (amount > 0)
                    {
                        TaxPercentRate = TaxRate.TaxPercentRound(shipmentTaxRate.Rate.Amount / amount);
                    }
                }

                TaxDetails = shipmentTaxRate.Line.TaxDetails;
            }
        }

        public void ApplyRewards(IEnumerable<PromotionReward> rewards)
        {
            var shipmentRewards = rewards
                .Where(r => r.RewardType == PromotionRewardType.ShipmentReward
                    && (string.IsNullOrEmpty(r.ShippingMethodCode) || r.ShippingMethodCode.EqualsInvariant(ShipmentMethodCode)));

            Discounts.Clear();

            DiscountAmount = new Money(0m, Currency);

            foreach (var reward in shipmentRewards)
            {
                var discount = reward.ToDiscountModel(Price - DiscountAmount);

                if (reward.IsValid && discount.Amount.InternalAmount > 0)
                {
                    Discounts.Add(discount);
                    DiscountAmount += discount.Amount;
                }
            }
        }

        /// <summary>
        /// Return true if the fields match
        /// </summary>
        public bool HasSameMethod(ShippingMethod method)
            => ShipmentMethodCode.EqualsInvariant(method.ShipmentMethodCode) && ShipmentMethodOption.EqualsInvariant(method.OptionName);

        public override object Clone()
        {
            var result = base.Clone() as Shipment;

            result.Price = Price.CloneAsMoney();
            result.PriceWithTax = PriceWithTax.CloneAsMoney();
            result.DiscountAmount = DiscountAmount.CloneAsMoney();
            result.DiscountAmountWithTax = DiscountAmountWithTax.CloneAsMoney();
            result.Total = Total.CloneAsMoney();
            result.TotalWithTax = TotalWithTax.CloneAsMoney();
            result.TaxTotal = TaxTotal.CloneAsMoney();

            if (Discounts != null)
            {
                result.Discounts = new List<Discount>(Discounts.Select(x => x.Clone() as Discount));
            }

            if (TaxDetails != null)
            {
                result.TaxDetails = new List<TaxDetail>(TaxDetails.Select(x => x.Clone() as TaxDetail));
            }

            if (Items != null)
            {
                result.Items = new List<CartShipmentItem>(Items.Select(x => x.Clone() as CartShipmentItem));
            }

            if (ValidationErrors != null)
            {
                result.ValidationErrors = new List<ValidationError>(ValidationErrors.Select(x => x.Clone() as ValidationError));
            }

            return result;
        }
    }
}
