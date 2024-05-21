using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class SeoInfosExtensions
    {
        public static SeoInfo GetBestMatchingSeoInfo(this IList<SeoInfo> seoInfos, string storeId, string cultureName)
        {
            if (storeId == null || cultureName == null)
            {
                return null;
            }

            return GetBestMatchingSeoInfoInternal(seoInfos, storeId, cultureName, null);
        }

        public static SeoInfo GetBestMatchingSeoInfo(this IList<SeoInfo> seoInfos, string storeId, string cultureName, string slug)
        {
            if (storeId == null || cultureName == null || slug == null)
            {
                return null;
            }

            return GetBestMatchingSeoInfoInternal(seoInfos, storeId, cultureName, slug);
        }

        private static SeoInfo GetBestMatchingSeoInfoInternal(IList<SeoInfo> seoInfos, string storeId, string cultureName, string slug)
        {
            return seoInfos
                .GetBestMatchingSeoInfos(storeId, cultureName, cultureName, slug);
        }

        public static SeoInfo GetFallbackSeoInfo(string id, string name, string cultureName)
        {
            var result = AbstractTypeFactory<SeoInfo>.TryCreateInstance();
            result.SemanticUrl = id;
            result.LanguageCode = cultureName;
            result.Name = name;
            return result;
        }

        private static SeoInfo GetBestMatchingSeoInfos(this IEnumerable<SeoInfo> seoRecords, string storeId, string storeDefaultLanguage, string language, string slug)
        {
            if (seoRecords != null)
            {
                var items = seoRecords
                    .Select(s =>
                    {
                        var score = 0;

                        score += s.IsActive != false ? 32 : 0;
                        score += !string.IsNullOrEmpty(slug) && slug.EqualsInvariant(s.SemanticUrl) ? 16 : 0;
                        score += storeId.EqualsInvariant(s.StoreId) ? 8 : 0;
                        score += language.Equals(s.LanguageCode) ? 4 : 0;
                        score += storeDefaultLanguage.EqualsInvariant(s.LanguageCode) ? 2 : 0;
                        score += s.LanguageCode.IsNullOrEmpty() ? 1 : 0;

                        return new { SeoRecord = s, Score = score };
                    })
                    .OrderByDescending(x => x.Score)
                    .ToList();

                var first = items.FirstOrDefault();
                return first?.SeoRecord;
            }

            return null;
        }
    }
}
