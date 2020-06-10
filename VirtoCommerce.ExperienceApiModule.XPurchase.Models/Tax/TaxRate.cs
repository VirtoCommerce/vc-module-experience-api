using System;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax
{
    public class TaxRate
    {
        public TaxRate(Currency currency) => Rate = new Money(currency);

        public Money Rate { get; set; }

        public decimal PercentRate { get; set; }

        public TaxLine Line { get; set; }

        public static decimal TaxPercentRound(decimal percent) => Math.Round(percent, 4, MidpointRounding.AwayFromZero);
    }
}
