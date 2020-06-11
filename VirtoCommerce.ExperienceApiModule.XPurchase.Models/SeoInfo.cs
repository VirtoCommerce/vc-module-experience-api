using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models
{
    /// <summary>
    /// Represent SEO information and contains common SEO fields
    /// </summary>
    public partial class SeoInfo : ValueObject, IHasLanguage
    {
        public string MetaDescription { get; set; }

        public string Slug { get; set; }

        public string MetaKeywords { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        #region IHasLanguage Members

        public Language Language { get; set; }

        #endregion IHasLanguage Members
    }
}
