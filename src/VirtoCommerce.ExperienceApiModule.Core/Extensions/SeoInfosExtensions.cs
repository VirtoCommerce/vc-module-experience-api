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
            var result = seoRecords?.Select(s => new
            {
                SeoRecord = s,
                Score = CalculateScore(s, slug, storeId, language, storeDefaultLanguage)
            })
            .OrderByDescending(x => x.Score)
            .Select(x => x.SeoRecord)
            .FirstOrDefault();

            return result;
        }

        private static int CalculateScore(SeoInfo seoInfo, string slug, string storeId, string language, string storeDefaultLanguage)
        {
            var score = 0;

            var values = new[]
            {
                seoInfo.IsActive,
                !string.IsNullOrEmpty(slug) && slug.EqualsInvariant(seoInfo.SemanticUrl),
                storeId.EqualsInvariant(seoInfo.StoreId),
                language.Equals(seoInfo.LanguageCode),
                storeDefaultLanguage.EqualsInvariant(seoInfo.LanguageCode),
                seoInfo.LanguageCode.IsNullOrEmpty()
            };

            for (var i = 0; i < values.Length; i++)
            {
                if (values[i])
                {
                    score += 1 << (values.Length - 1 - i);
                }
            }

            return score;
        }
    }
}
