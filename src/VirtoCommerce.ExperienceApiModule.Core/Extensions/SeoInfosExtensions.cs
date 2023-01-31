using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Tools;

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

            return seoInfos
                ?.Select(x => JObject.FromObject(x).ToObject<Tools.Models.SeoInfo>())
                .GetBestMatchingSeoInfos(storeId, cultureName, cultureName, null)
                .Select(x =>
                {
                    x.CreatedDate ??= DateTime.MinValue;
                    x.ModifiedDate ??= DateTime.Now;
                    return x;
                })
                .Select(x => JObject.FromObject(x).ToObject<SeoInfo>())
                .FirstOrDefault();
        }

        public static SeoInfo GetFallbackSeoInfo(string id, string name, string cultureName)
        {
            var result = AbstractTypeFactory<SeoInfo>.TryCreateInstance();
            result.SemanticUrl = id;
            result.LanguageCode = cultureName;
            result.Name = name;
            return result;
        }
    }
}
