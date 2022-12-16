using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class IHasLanguageExtensions
    {
        /// <summary>
        /// Looking for first best-match language-specific value in the enumerable, based on specified language value selector function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hasLanguages">An enumerable with values to search</param>
        /// <param name="langSelector">Selector function, returning the language of each enumerable item</param>
        /// <param name="language">Language to search</param>
        /// <returns>First matching item to the specified language</returns>
        public static T FirstBestMatchForLanguage<T>(this IEnumerable<T> hasLanguages, Func<T, string> langSelector, string language)
        {
            if (hasLanguages == null)
            {
                throw new ArgumentNullException(nameof(hasLanguages));
            }
            //Try find object for passed language event if it null
            var result = hasLanguages.FirstOrDefault(x => langSelector(x)?.EqualsInvariant(language) ?? langSelector(x) == language);
            if (result == null)
            {
                //find the first with no language set
                result = hasLanguages.FirstOrDefault(x => langSelector(x) == null);
            }
            return result;
        }

        /// <summary>
        /// Looking for first best-match language-specific value in the enumerable
        /// </summary>
        /// <param name="hasLanguages">An enumerable with values to search</param>
        /// <param name="language">Language to search</param>
        /// <returns>First matching item to the specified language</returns>
        public static IHasLanguage FirstBestMatchForLanguage(this IEnumerable<IHasLanguage> hasLanguages, string language)
        {
            return hasLanguages.FirstBestMatchForLanguage(x => x.LanguageCode, language);
        }
    }
}
