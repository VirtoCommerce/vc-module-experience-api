using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.XDigitalCatalog.Interfaces;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Index
{
    //TODO: Need to think about extensibility becasue static class do not extensible
    public static class IndexFieldMapper
    {
        public static bool HasPricingFields(this IHasIncludeFields hasIncludeFields)
        {
            return hasIncludeFields.IncludeFields.Any(x => x.Contains("prices", StringComparison.OrdinalIgnoreCase));
        }
        public static bool HasInventoryFields(this IHasIncludeFields hasIncludeFields)
        {
            return hasIncludeFields.IncludeFields.Any(x => x.Contains("availabilityData", StringComparison.OrdinalIgnoreCase));
        }
        public static string[] MapToIndexFields(this IHasIncludeFields hasIncludeFields)
        {
            var includeFields = hasIncludeFields.IncludeFields;
            /*
             * TODO: refactor this to implement "context" building which contains different result for different search engines and
             * all conditional filelds like LoadPrices and LoadInventories
             */

            var result = new List<string>();

            // Add filds for __object
            result.AddRange(includeFields.Concat(new[] { "id" }).Select(x => "__object." + x));

            if (hasIncludeFields.HasPricingFields())
            {
                result.Add("__prices");
            }

            if (includeFields.Any(x => x.Contains("variations", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__variations");
            }

            if (includeFields.Any(x => x.StartsWith("category", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.categoryId");
            }

            // Add master variation fields
            result.AddRange(includeFields.Where(x => x.StartsWith("masterVariation."))
                                         .Concat(new[] { "mainProductId" })
                                         .Select(x => "__object." + x.TrimStart("masterVariation.")));

            // Add metaKeywords, metaTitle and metaDescription
            if (includeFields.Any(x => x.Contains("slug", StringComparison.OrdinalIgnoreCase) || x.Contains("meta", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.seoInfos");
            }

            if (includeFields.Any(x => x.Contains("imgSrc", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.images");
            }

            if (includeFields.Any(x => x.Contains("brandName", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.properties");
            }

            if (includeFields.Any(x => x.Contains("descriptions", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.reviews");
            }

            if (hasIncludeFields.HasInventoryFields())
            {
                result.Add("__object.isActive");
                result.Add("__object.isBuyable");
                result.Add("__object.trackInventory");
            }

            return result.Distinct().ToArray();
        }
    }
}
