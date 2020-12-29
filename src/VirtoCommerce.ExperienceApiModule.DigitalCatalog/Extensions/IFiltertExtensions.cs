using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class IFiltertExtensions
    {
        /// <summary>
        /// Checks aggregation item values for the equality with the filters, and set <see cref="AggregationItem.IsApplied"/> to those, whose value equal to  on of the filters
        /// </summary>
        /// <param name="searchRequest">Search request</param>
        /// <param name="aggregations">Calculated aggregation results</param>
        public static void SetAppliedAggregations(this SearchRequest searchRequest, Aggregation[] aggregations)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest));
            }
            if (aggregations == null)
            {
                throw new ArgumentNullException(nameof(aggregations));
            }

            foreach (var childFilter in searchRequest.GetChildFilters())
            {
                var aggregationItems = aggregations.Where(x => x.Field.EqualsInvariant(childFilter.GetFieldName()))
                    .SelectMany(x => x.Items)
                    .ToArray();

                childFilter.FillIsAppliedForItems(aggregationItems);
            }
        }

        public static IList<IFilter> GetChildFilters(this SearchRequest searchRequest) =>
            (searchRequest?.Filter as AndFilter)?.ChildFilters ?? Array.Empty<IFilter>();

        public static string GetFieldName(this IFilter filter) =>
            // TermFilter names are equal, RangeFilter can contain underscore in the name
            (filter as INamedFilter)?.FieldName?.Split('_')[0];

        public static void FillIsAppliedForItems(this IFilter filter, IEnumerable<AggregationItem> aggregationItems)
        {
            foreach (var aggregationItem in aggregationItems)
            {
                switch (filter)
                {
                    case TermFilter termFilter:
                        // For term filters: just check result value in filter values
                        aggregationItem.IsApplied = termFilter.Values.Any(x => x.EqualsInvariant(aggregationItem.Value?.ToString()));
                        break;
                    case RangeFilter rangeFilter:
                        // For range filters check the values have the same bounds
                        aggregationItem.IsApplied = rangeFilter.Values.Any(x =>
                            x.Lower.EqualsInvariant(aggregationItem.RequestedLowerBound) && x.Upper.EqualsInvariant(aggregationItem.RequestedUpperBound));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
