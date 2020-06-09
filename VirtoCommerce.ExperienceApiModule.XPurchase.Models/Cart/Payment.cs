using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Extensions;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart
{
    public class Payment : CloneableEntity, ITaxable, IDiscountable
    {
        public Payment(Currency currency)
        {
            Discounts = new List<Discount>();
            TaxDetails = new List<TaxDetail>();

            Currency = currency;
            Price = new Money(currency);
            PriceWithTax = new Money(currency);
            DiscountAmount = new Money(currency);
            DiscountAmountWithTax = new Money(currency);
            Amount = new Money(currency);
            Total = new Money(currency);
            TotalWithTax = new Money(currency);
            TaxTotal = new Money(currency);
        }

        /// <summary>
        /// Gets or sets the value of payment outer id.
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// Gets or sets the value of payment gateway code.
        /// </summary>
        public string PaymentGatewayCode { get; set; }

        /// <summary>
        /// Gets or sets the value of payment currency.
        /// </summary>
        /// <value>
        /// Currency code in ISO 4217 format.
        /// </value>
        public Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets the value of payment amount.
        /// </summary>
        public Money Amount { get; set; }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        /// <value>
        /// Address object.
        /// </value>
        public Address BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the value of payment service price.
        /// </summary>
        public Money Price { get; set; }

        /// <summary>
        /// Gets or sets the value of payment service price including tax
        /// Price * TaxPercentRate.
        /// </summary>
        public Money PriceWithTax { get; set; }

        /// <summary>
        /// Gets the value of total payment service price without taxes
        /// Price - DiscountAmount;.
        /// </summary>
        public Money Total { get; set; }

        /// <summary>
        /// Gets the value of total payment service price including taxes
        /// Total * TaxPercentRate.
        /// </summary>
        public Money TotalWithTax { get; set; }

        /// <summary>
        /// Gets the value of total payment service discount amount.
        /// </summary>
        public Money DiscountAmount { get; set; }

        /// <summary>
        /// DiscountAmount * TaxPercentRate.
        /// </summary>
        public Money DiscountAmountWithTax { get; set; }


        /// <summary>
        /// Gets or sets the value of total payment service tax amount.
        /// </summary>
        public Money TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        /// <summary>
        /// Gets or sets the value of payment tax type.
        /// </summary>
        public string TaxType { get; set; }

        /// <summary>
        /// Gets or sets the collection of line item tax details lines.
        /// </summary>
        /// <value>
        /// Collection of TaxDetail objects.
        /// </value>
        public IList<TaxDetail> TaxDetails { get; set; }

        public IList<Discount> Discounts { get; private set; }

        public void ApplyTaxRates(IEnumerable<TaxRate> taxRates)
        {
            TaxPercentRate = 0m;
            var taxRatesList = taxRates.ToList();
            var paymentTaxRate = taxRatesList.FirstOrDefault(x => x.Line.Id != null && x.Line.Id.EqualsInvariant(Id ?? string.Empty));
            if (paymentTaxRate == null)
            {
                paymentTaxRate = taxRatesList.FirstOrDefault(x => x.Line.Code.EqualsInvariant(PaymentGatewayCode));
            }

            if (paymentTaxRate != null)
            {
                if (paymentTaxRate.PercentRate > 0)
                {
                    TaxPercentRate = paymentTaxRate.PercentRate;
                }
                else
                {
                    var amount = Total.Amount > 0 ? Total.Amount : Price.Amount;
                    if (amount > 0)
                    {
                        TaxPercentRate = TaxRate.TaxPercentRound(paymentTaxRate.Rate.Amount / amount);
                    }
                }

                TaxDetails = paymentTaxRate.Line.TaxDetails;
            }
        }

        public void ApplyRewards(IEnumerable<PromotionReward> rewards)
        {
            var paymentRewards = rewards.Where(r => r.RewardType == PromotionRewardType.PaymentReward && (r.PaymentMethodCode.IsNullOrEmpty() || r.PaymentMethodCode.EqualsInvariant(PaymentGatewayCode)));

            Discounts.Clear();

            DiscountAmount = new Money(0m, Currency);

            foreach (var reward in paymentRewards)
            {
                var discount = reward.ToDiscountModel(Price - DiscountAmount);

                if (reward.IsValid && discount.Amount.InternalAmount > 0)
                {
                    Discounts.Add(discount);
                    DiscountAmount += discount.Amount;
                }
            }
        }

        public override object Clone()
        {
            var result = base.Clone() as Payment;

            result.Currency = result.CloneAsCurrency();
            result.Price = result.Price.CloneAsMoney();
            result.PriceWithTax = result.PriceWithTax.CloneAsMoney();
            result.DiscountAmount = result.DiscountAmount.CloneAsMoney();
            result.DiscountAmountWithTax = result.DiscountAmountWithTax.CloneAsMoney();
            result.Amount = result.Amount.CloneAsMoney();
            result.Total = result.Total.CloneAsMoney();
            result.TotalWithTax = result.TotalWithTax.CloneAsMoney();
            result.TaxTotal = result.TaxTotal.CloneAsMoney();

            if (Discounts != null)
            {
                result.Discounts = new List<Discount>(Discounts.Select(x => x.Clone() as Discount));
            }

            if (TaxDetails != null)
            {
                result.TaxDetails = new List<TaxDetail>(TaxDetails.Select(x => x.Clone() as TaxDetail));
            }

            return result;
        }
    }
}
