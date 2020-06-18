using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.Tax;
using ValueObject = VirtoCommerce.XPurchase.Models.Common.ValueObject;

namespace VirtoCommerce.XPurchase.Models
{
    public partial class TierPrice : ValueObject, ITaxable
    {
        public TierPrice(Currency currency)
            : this(new Money(currency), 0)
        {
        }
        public TierPrice(Money price, long quantity)
        {
            Currency = price.Currency;
            TaxDetails = new List<TaxDetail>();
            Price = price;
            DiscountAmount = new Money(price.Currency);

            Quantity = quantity;
        }

        public Money Price { get; set; }

        private Money _price;
        public Money PriceWithTax
        {
            get => _price ?? Price;
            set => _price = value;
        }

        public Money DiscountAmount { get; set; }
        public Money DiscountAmountWithTax => DiscountAmount + (DiscountAmount * TaxPercentRate);

        /// <summary>
        /// Actual price includes all kind of discounts
        /// </summary>
        public Money ActualPrice => Price - DiscountAmount;

        public Money ActualPriceWithTax => PriceWithTax - DiscountAmountWithTax;

        public long Quantity { get; set; }



        #region ITaxable Members
        public Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets the value of total shipping tax amount
        /// </summary>
        public Money TaxTotal => ActualPriceWithTax - ActualPrice;

        public decimal TaxPercentRate { get; private set; }

        /// <summary>
        /// Gets or sets the value of shipping tax type
        /// </summary>
        public string TaxType { get; set; }

        /// <summary>
        /// Gets or sets the collection of line item tax details lines
        /// </summary>
        /// <value>
        /// Collection of TaxDetail objects
        /// </value>
        public IList<TaxDetail> TaxDetails { get; set; }

        public void ApplyTaxRates(IEnumerable<TaxRate> taxRates)
        {
            var shipmentTaxRate = taxRates.FirstOrDefault(x => x.Line.Quantity == Quantity);
            if (shipmentTaxRate != null)
            {
                if (shipmentTaxRate.PercentRate > 0)
                {
                    TaxPercentRate = shipmentTaxRate.PercentRate;
                }
                else
                {
                    if (ActualPrice.Amount > 0)
                    {
                        TaxPercentRate = TaxRate.TaxPercentRound(shipmentTaxRate.Rate.Amount / ActualPrice.Amount);
                    }
                }

                TaxDetails = shipmentTaxRate.Line.TaxDetails;
            }
        }
        #endregion

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Price;
            yield return DiscountAmount;
            yield return TaxPercentRate;
            yield return Quantity;

        }
    }
}
