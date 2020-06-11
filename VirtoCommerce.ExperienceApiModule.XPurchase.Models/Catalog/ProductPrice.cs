using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog
{
    public partial class ProductPrice : ValueObject, IConvertible<ProductPrice>, ITaxable
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
        public string ProductId { get; set; }


        public Money DiscountAmount { get; set; }

        public Money DiscountAmountWithTax => DiscountAmount + (DiscountAmount * TaxPercentRate);

        /// <summary>
        /// Relative benefit. 30% 
        /// </summary>
        public decimal DiscountPercent
            => ListPrice.Amount > 0
                ? Math.Round(DiscountAmount.Amount / ListPrice.Amount, 2)
                : 0;
                
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

        public IList<Discount> Discounts { get; set; }

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
        public TierPrice GetTierPrice(int quantity)
            => TierPrices
                .OrderBy(x => x.Quantity)
                .LastOrDefault(x => x.Quantity <= quantity)
            ?? new TierPrice(SalePrice, 1);

        #region ITaxable Members
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
            var taxRate = taxRates.FirstOrDefault(x => x.Line.Quantity == 0);

            if (taxRate != null)
            {
                if (taxRate.PercentRate > 0)
                {
                    TaxPercentRate = taxRate.PercentRate;
                }
                else if(ActualPrice.Amount > 0)
                {
                    TaxPercentRate = TaxRate.TaxPercentRound(taxRate.Rate.Amount / ActualPrice.Amount);
                }
                else if(SalePrice.Amount > 0)
                {
                    TaxPercentRate = TaxRate.TaxPercentRound(taxRate.Rate.Amount / SalePrice.Amount);
                }

                TaxDetails = taxRate.Line.TaxDetails;
            }

            foreach (var tierPrice in TierPrices)
            {
                tierPrice.ApplyTaxRates(taxRates);
            }
        }
        #endregion

        #region IConvertible<ProductPrice> Members
        /// <summary>
        /// Convert current product price to other currency using currency exchange rate
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public ProductPrice ConvertTo(Currency currency)
        {
            var retVal = new ProductPrice(currency)
            {
                ListPrice = ListPrice.ConvertTo(currency),
                SalePrice = SalePrice.ConvertTo(currency),
                DiscountAmount = DiscountAmount.ConvertTo(currency),
                ProductId = ProductId
            };

            return retVal;
        }
        #endregion

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
