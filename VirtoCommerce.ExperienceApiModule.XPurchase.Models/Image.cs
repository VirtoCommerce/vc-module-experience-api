using System.Collections.Generic;
using ValueObject = VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common.ValueObject;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models
{
    public partial class Image : ValueObject
    {
        /// <summary>
        /// Full url of image
        /// </summary>
        public string Url { get; set; }

        public string FullSizeImageUrl { get; set; }

        /// <summary>
        /// Image title
        /// </summary>
        public string Title { get; set; }
        public string Name => Title;

        /// <summary>
        /// Image alt text
        /// </summary>
        public string Alt { get; set; }

        public int? SortOrder { get; set; }
        public string Group { get; set; }
        public string LanguageCode { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
        }
    }
}
