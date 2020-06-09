using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax
{
    public partial class TaxDetail : CloneableValueObject
    {
        public TaxDetail(Currency currency)
        {
            Rate = new Money(currency);
            Amount = new Money(currency);
        }

        public Money Rate { get; set; }

        public Money Amount { get; set; }

        public string Title => Name;

        public decimal Price => Amount.Amount * 100;

        public string Name { get; set; }

        public override object Clone()
        {
            var result = base.Clone() as TaxDetail;
            result.Rate = Rate.CloneAsMoney();
            result.Amount = Amount.CloneAsMoney();
            return result;
        }
    }
}
