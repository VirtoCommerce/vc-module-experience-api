using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Tools;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class SeoInfosExtensions
    {
        public static CoreModule.Core.Seo.SeoInfo GetBestMatchingSeoInfo(this IList<CoreModule.Core.Seo.SeoInfo> seoInfos, string storeId, string cultureName)
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
                .Select(x => JObject.FromObject(x).ToObject<CoreModule.Core.Seo.SeoInfo>())
                .FirstOrDefault();
        }
    }
}
