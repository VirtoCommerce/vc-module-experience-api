using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models
{
    public class LocalizedString : IHasLanguage
    {
        public LocalizedString()
        {
            Language = Language.InvariantLanguage;
            Value = null;
        }
        public LocalizedString(Language language, string value)
        {
            Language = language;
            Value = value;

        }

        public string Value { get; set; }

        #region IHasLanguage Members
        public Language Language { get; set; }
        #endregion
    }
}
