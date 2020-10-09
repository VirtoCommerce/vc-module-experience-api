using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class IHasLanguageExtensions
    {
        public static IHasLanguage FirstBestMatchForLanguage(this IEnumerable<IHasLanguage> hasLanguages, string language)
        {
            if (hasLanguages == null)
            {
                throw new ArgumentNullException(nameof(hasLanguages));
            }
            //Try find object for passed language event if it null
            var result = hasLanguages.FirstOrDefault(x => x.LanguageCode?.EqualsInvariant(language) ?? x.LanguageCode == language);
            if (result == null)
            {
                //find the first with no language set
                result = hasLanguages.FirstOrDefault(x => x.LanguageCode == null);
            }
            return result;

        }
    }
}
