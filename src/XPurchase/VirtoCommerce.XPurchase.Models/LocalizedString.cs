using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models
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
