using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase
{
    public partial class TierPrice : ValueObject, ITaxable, IHasTaxDetalization
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
            get
            {
                return _price ?? Price;
            }
            set
            {
                _price = value;
            }
        }

        public Money DiscountAmount { get; set; }
        public Money DiscountAmountWithTax
        {
            get
            {
                return DiscountAmount + DiscountAmount * TaxPercentRate;
            }
        }

        /// <summary>
        /// Actual price includes all kind of discounts
        /// </summary>
        public Money ActualPrice
        {
            get
            {
                return Price - DiscountAmount;
            }
        }

        public Money ActualPriceWithTax
        {
            get
            {
                return PriceWithTax - DiscountAmountWithTax;
            }
        }

        public long Quantity { get; set; }


        public Currency Currency { get; set; }

        #region ITaxable Members
        decimal ITaxable.TaxTotal => TaxTotal.Amount;
        /// <summary>
        /// Gets or sets the value of total shipping tax amount
        /// </summary>
        public Money TaxTotal
        {
            get
            {
                return ActualPriceWithTax - ActualPrice;
            }
        }

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
        public ICollection<TaxDetail> TaxDetails { get; set; }


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
