using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Extensions;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart
{
    public class LineItem : CloneableEntity, IDiscountable, IValidatable, ITaxable
    {
        public LineItem(Currency currency, Language language)
        {
            Currency = currency;
            LanguageCode = language.CultureName;
            ListPrice = new Money(currency);
            SalePrice = new Money(currency);
            DiscountAmount = new Money(currency);
            DiscountAmountWithTax = new Money(currency);
            DiscountTotal = new Money(currency);
            DiscountTotalWithTax = new Money(currency);
            ListPriceWithTax = new Money(currency);
            SalePriceWithTax = new Money(currency);
            PlacedPrice = new Money(currency);
            PlacedPriceWithTax = new Money(currency);
            ExtendedPrice = new Money(currency);
            ExtendedPriceWithTax = new Money(currency);
            TaxTotal = new Money(currency);
            Discounts = new List<Discount>();
            TaxDetails = new List<TaxDetail>();
            ValidationErrors = new List<ValidationError>();
            IsValid = true;
        }

        /// <summary>
        /// Gets or sets line item created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the product corresponding to line item
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the value of product id.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the type of product (can be Physical, Digital or Subscription).
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// Gets or sets the value of catalog id.
        /// </summary>
        public string CatalogId { get; set; }

        /// <summary>
        /// Gets or sets the value of category id.
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the value of product SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the value of line item name.
        /// </summary>
        public string Name { get; set; }

        [JsonIgnore]
        public string Title => Name;

        /// <summary>
        /// Gets or sets the value of line item quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or Sets InStockQuantity.
        /// </summary>
        public int InStockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the value of line item warehouse location.
        /// </summary>
        public string WarehouseLocation { get; set; }

        /// <summary>
        /// Gets or sets the value of line item shipping method code.
        /// </summary>
        public string ShipmentMethodCode { get; set; }

        /// <summary>
        /// Gets or sets the requirement for line item shipping.
        /// </summary>
        public bool RequiredShipping { get; set; }

        /// <summary>
        /// Gets or sets the value of line item thumbnail image absolute URL.
        /// </summary>
        public string ThumbnailImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the value of line item image absolute URL.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the flag of line item is a gift.
        /// </summary>
        public bool IsGift { get; set; }

        /// <summary>
        /// Gets the value of language code.
        /// </summary>
        /// <value>
        /// Culture name in ISO 3166-1 alpha-3 format.
        /// </value>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// Gets or sets the value of line item comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the flag of line item is recurring.
        /// </summary>
        public bool IsReccuring { get; set; }

        /// <summary>
        /// Gets or sets flag of line item has tax.
        /// </summary>
        public bool TaxIncluded { get; set; }

        /// <summary>
        /// Gets or sets the value of line item volumetric weight.
        /// </summary>
        public decimal? VolumetricWeight { get; set; }

        /// <summary>
        /// Gets or sets the value of line item weight unit.
        /// </summary>
        public string WeightUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of line item weight.
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// Gets or sets the value of line item measurement unit.
        /// </summary>
        public string MeasureUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of line item height.
        /// </summary>
        public decimal? Height { get; set; }

        /// <summary>
        /// Gets or sets the value of line item length.
        /// </summary>
        public decimal? Length { get; set; }

        /// <summary>
        /// Gets or sets the value of line item width.
        /// </summary>
        public decimal? Width { get; set; }

        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the value of line item original price.
        /// </summary>
        public Money ListPrice { get; set; }

        /// <summary>
        /// Gets or sets the value of line item original price including tax
        /// ListPrice * TaxPercentRate;.
        /// </summary>
        public Money ListPriceWithTax { get; set; }

        /// <summary>
        /// Gets or sets the value of line item sale price.
        /// </summary>
        public Money SalePrice { get; set; }

        /// <summary>
        /// Gets or sets the value of line item sale price with tax
        /// SalePrice * TaxPercentRate;.
        /// </summary>
        public Money SalePriceWithTax { get; set; }

        /// <summary>
        /// Gets or sets the value of line item actual price (include all types of discounts)
        /// ListPrice - DiscountAmount;.
        /// </summary>
        public Money PlacedPrice { get; set; }

        /// <summary>
        /// Gets or sets the value of line item actual price (include tax)
        /// PlacedPrice * TaxPercentRate.
        /// </summary>
        public Money PlacedPriceWithTax { get; set; }

        /// <summary>
        /// Gets or sets the value of line item extended price
        /// PlacedPrice * Quantity;.
        /// </summary>
        public Money ExtendedPrice { get; set; }

        /// <summary>
        /// Gets or sets the value of line item extended price (include tax)
        /// ExtendedPrice * TaxPercentRate.
        /// </summary>
        public Money ExtendedPriceWithTax { get; set; }

        public Money DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the value of line item discount amount (include tax)
        /// DiscountAmount  * TaxPercentRate.
        /// </summary>
        public Money DiscountAmountWithTax { get; set; }

        /// <summary>
        /// Gets or sets the value of line item total discount amount
        /// DiscountAmount * Math.Max(1, Quantity);.
        /// </summary>
        public Money DiscountTotal { get; set; }

        /// <summary>
        /// Gets or sets the value of line item total discount amount (include tax)
        /// DiscountTotal * TaxPercentRate.
        /// </summary>
        public Money DiscountTotalWithTax { get; set; }

        /// <summary>
        /// Used for dynamic properties management, contains object type string.
        /// </summary>
        /// <value>Contains object type string.</value>
        public string ObjectType { get; set; }

        public bool IsValid { get; set; }

        public IList<ValidationError> ValidationErrors { get; set; }

        /// <summary>
        /// Gets or sets the value of total shipping tax amount.
        /// </summary>
        public virtual Money TaxTotal { get; set; }

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

        public Currency Currency { get; private set; }

        public IList<Discount> Discounts { get; private set; }

        public void ApplyTaxRates(IEnumerable<TaxRate> taxRates)
        {
            TaxPercentRate = 0m;
            var taxRatesList = taxRates.ToList();
            var taxRate = taxRatesList.FirstOrDefault(x => x.Line.Id != null && x.Line.Id.EqualsInvariant(Id ?? string.Empty))
                       ?? taxRatesList.FirstOrDefault(x => x.Line.Code != null && x.Line.Code.EqualsInvariant(Sku ?? string.Empty));

            if (taxRate == null)
            {
                return;
            }

            if (taxRate.PercentRate > 0)
            {
                TaxPercentRate = taxRate.PercentRate;
            }
            else
            {
                var amount = ExtendedPrice.Amount > 0 ? ExtendedPrice.Amount : SalePrice.Amount;
                if (amount > 0)
                {
                    TaxPercentRate = TaxRate.TaxPercentRound(taxRate.Rate.Amount / amount);
                }
            }

            TaxDetails = taxRate.Line.TaxDetails;
        }

        public void ApplyRewards(IEnumerable<PromotionReward> rewards)
        {
            var lineItemRewards = rewards
                .Where(x => x.RewardType == PromotionRewardType.CatalogItemAmountReward
                    && (x.ProductId.IsNullOrEmpty() || x.ProductId.EqualsInvariant(ProductId)));

            Discounts.Clear();

            DiscountAmount = new Money(Math.Max(0, (ListPrice - SalePrice).Amount), Currency);

            if (Quantity == 0)
            {
                return;
            }

            foreach (var reward in lineItemRewards)
            {
                if (reward.IsValid)
                {
                    var discount = reward.ToDiscountModel(ListPrice - DiscountAmount, Quantity);
                    if (discount.Amount.InternalAmount > 0)
                    {
                        Discounts.Add(discount);
                        DiscountAmount += discount.Amount;
                    }
                }
            }
        }

        public override string ToString() => $"cart lineItem #{Id ?? "undef"} {Name ?? "undef"} qty: {Quantity}";

        public override object Clone()
        {
            var result = base.Clone() as LineItem;

            result.ListPrice = ListPrice.CloneAsMoney();
            result.SalePrice = SalePrice.CloneAsMoney();
            result.DiscountAmount = DiscountAmount.CloneAsMoney();
            result.DiscountAmountWithTax = DiscountAmountWithTax.CloneAsMoney();
            result.DiscountTotal = DiscountTotal.CloneAsMoney();
            result.DiscountTotalWithTax = DiscountTotalWithTax.CloneAsMoney();
            result.ListPriceWithTax = ListPriceWithTax.CloneAsMoney();
            result.SalePriceWithTax = SalePriceWithTax.CloneAsMoney();
            result.PlacedPrice = PlacedPrice.CloneAsMoney();
            result.PlacedPriceWithTax = PlacedPriceWithTax.CloneAsMoney();
            result.ExtendedPrice = ExtendedPrice.CloneAsMoney();
            result.ExtendedPriceWithTax = ExtendedPriceWithTax.CloneAsMoney();
            result.TaxTotal = TaxTotal.CloneAsMoney();

            if (Discounts != null)
            {
                result.Discounts = new List<Discount>(Discounts.Select(x => x.Clone() as Discount));
            }

            if (TaxDetails != null)
            {
                result.TaxDetails = new List<TaxDetail>(TaxDetails.Select(x => x.Clone() as TaxDetail));
            }

            if (ValidationErrors != null)
            {
                result.ValidationErrors = new List<ValidationError>(ValidationErrors.Select(x => x.Clone() as ValidationError));
            }

            return result;
        }
    }
}
