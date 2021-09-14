using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class FacetsExtensions
    {
        /// <summary>
        /// Add to the facet phrase language-specific facet name in a hope the sought facet can be made by non-dictionary, multivalue and multilanguage property.
        /// See details: PT-3517
        /// </summary>
        /// <param name="requestFacets"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static string AddLanguageSpecificFacets(this string requestFacets, string languageCode)
        {
            var facets = string.Empty;
            if (languageCode != null)
            {
                foreach (var facet in requestFacets?.Split(" ") ?? new string[0])
                {
                    facets = $"{facets} {facet} {facet}_{languageCode.ToLowerInvariant()}";
                }
            }
            return facets.Trim();
        }

        /// <summary>
        /// Apply language-specific facet result
        /// See details: PT-3517
        /// </summary>
        /// <param name="aggregations"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static IEnumerable<Aggregation> ApplyLanguageSpecificFacetResult(this Aggregation[] aggregations, string languageCode)
        {
            return aggregations?.Select(x =>
            {
                // Apply language-specific facet result
                // To do this, copy facet items from the fake language-specific facet to the real facet
                var languageSpecificAggregation = aggregations.FirstOrDefault(y => y.Field == $"{x.Field}_{languageCode.ToLowerInvariant()}");
                if (languageSpecificAggregation != null)
                    x.Items = languageSpecificAggregation.Items;
                return x;
            })
            .Where(x => !Regex.IsMatch(x.Field, @"_\w\w-\w\w$", RegexOptions.IgnoreCase)); // Drop fake language-specific facets from results
        }
    }
}
