using System.Collections.Generic;
using System.Globalization;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core
{
    // TODO: Move to Core module
    public class Language : ValueObject
    {
        public Language(string cultureName)
        {
            var culture = string.IsNullOrEmpty(cultureName) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(cultureName);

            CultureName = culture.Name;
            ThreeLeterLanguageName = culture.ThreeLetterISOLanguageName;
            TwoLetterLanguageName = culture.TwoLetterISOLanguageName;
            NativeName = culture.NativeName;
            if (culture != CultureInfo.InvariantCulture)
            {
                var regionInfo = new RegionInfo(culture.LCID);
                TwoLetterRegionName = regionInfo.TwoLetterISORegionName;
                ThreeLetterRegionName = regionInfo.ThreeLetterISORegionName;
            }
        }

        private Language()
            : this(CultureInfo.InvariantCulture.Name)
        {
        }

        public static Language InvariantLanguage => new Language();

        public bool IsInvariant => CultureName == CultureInfo.InvariantCulture.Name;

        /// <summary>
        /// culture name format (e.g. en-US).
        /// </summary>
        public string CultureName { get; private set; }

        public string NativeName { get; private set; }

        /// <summary>
        ///  Gets the ISO 639-2 three-letter code for the language.
        /// </summary>
        public string ThreeLeterLanguageName { get; private set; }

        /// <summary>
        ///   Gets the ISO 639-1 two-letter code for the language.
        /// </summary>
        public string TwoLetterLanguageName { get; private set; }

        /// <summary>
        ///  Gets the two-letter code defined in ISO 3166 for the country/region.
        /// </summary>
        public string TwoLetterRegionName { get; private set; }

        /// <summary>
        ///  Gets the three-letter code defined in ISO 3166 for the country/region.
        /// </summary>
        public string ThreeLetterRegionName { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CultureName;
        }
    }
}
