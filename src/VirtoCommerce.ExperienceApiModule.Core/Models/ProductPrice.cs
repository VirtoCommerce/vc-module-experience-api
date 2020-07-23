using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Models
{
    public partial class ProductPrice : ValueObject, ITaxable, IHasTaxDetalization, IHasDiscounts
    {
        public ProductPrice(Currency currency)
        {
            Currency = currency;
            ListPrice = new Money(currency);
            SalePrice = new Money(currency);
            DiscountAmount = new Money(currency);
            TierPrices = new List<TierPrice>();
            Discounts = new List<Discount>();
        }

        /// <summary>
        /// Price list id
        /// </summary>
        public string PricelistId { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// Product id
        /// </summary>
        public string ProductId { get; set; }

        public Money DiscountAmount { get; set; }

        public Money DiscountAmountWithTax => DiscountAmount + (DiscountAmount * TaxPercentRate);

        /// <summary>
        /// Relative benefit. 30%
        /// </summary>
        public decimal DiscountPercent => ListPrice.Amount > 0 ? Math.Round(DiscountAmount.Amount / ListPrice.Amount, 2) : 0;

        /// <summary>
        /// Original product price (old price)
        /// </summary>
        public Money ListPrice { get; set; }

        /// <summary>
        /// Original product price (old price) including tax
        /// </summary>
        public Money ListPriceWithTax => ListPrice + (ListPrice * TaxPercentRate);

        /// <summary>
        /// Sale product price (new price)
        /// </summary>
        public Money SalePrice { get; set; }

        /// <summary>
        /// Sale product price (new price) including tax
        /// </summary>
        public Money SalePriceWithTax => SalePrice + (SalePrice * TaxPercentRate);

        /// <summary>
        /// Actual price includes all kind of discounts
        /// </summary>
        public Money ActualPrice => ListPrice - DiscountAmount;

        /// <summary>
        /// Actual price includes all kind of discounts including tax
        /// </summary>
        public Money ActualPriceWithTax => ListPriceWithTax - DiscountAmountWithTax;

        /// <summary>
        /// It defines the minimum quantity of products
        /// </summary>
        public int? MinQuantity { get; set; }

        /// <summary>
        /// Tier prices
        /// </summary>
        public IList<TierPrice> TierPrices { get; set; }

        /// <summary>
        /// Return tire price for passed quantity
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public TierPrice GetTierPrice(int quantity)
        {
            var retVal = TierPrices.OrderBy(x => x.Quantity).LastOrDefault(x => x.Quantity <= quantity);
            if (retVal == null)
            {
                retVal = new TierPrice(SalePrice, 1);
            }

            return retVal;
        }

        #region ITaxable Members

        decimal ITaxable.TaxTotal => TaxTotal.Amount;

        /// <summary>
        /// Gets or sets the value of total shipping tax amount
        /// </summary>
        public Money TaxTotal => ActualPriceWithTax - ActualPrice;

        public decimal TaxPercentRate { get; set; }

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

        #endregion ITaxable Members

        public ICollection<Discount> Discounts { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProductId;
            yield return Currency;
            yield return MinQuantity;
            yield return ListPrice;
            yield return SalePrice;
            yield return DiscountAmount;
            yield return PricelistId;
            yield return TaxPercentRate;

            if (TierPrices != null)
            {
                foreach (var tierPrice in TierPrices)
                {
                    yield return tierPrice;
                }
            }
        }
    }
}
