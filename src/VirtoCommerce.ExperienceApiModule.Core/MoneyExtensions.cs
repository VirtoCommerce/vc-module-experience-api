using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ExperienceApiModule.Core
{
    //TODO: Remove later, only as temporary workaround, need to costruct money from proper store currency and given language
    public static class MoneyExtensions
    {
        public static Money ToMoney(this decimal amount, string currency)
        {
            return new Money(amount, new Currency(Language.InvariantLanguage, currency));
        }
    }
}
