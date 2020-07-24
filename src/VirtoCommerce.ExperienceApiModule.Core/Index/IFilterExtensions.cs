using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Index
{
    public static class IFilterExtensions
    {
        public static string Stringify(this IFilter filter)
        {
            var result = filter.ToString();
            if (filter is RangeFilter rangeFilter)
            {
                result = rangeFilter.FieldName + "_" + string.Join("_", rangeFilter.Values.Select(Stringify));
            }
            else if (filter is TermFilter termFilter)
            {
                result = termFilter.FieldName + (!termFilter.Values.IsNullOrEmpty() ? string.Join("_", termFilter.Values) : string.Empty);
            }
            return result;
        }

        public static string Stringify(this RangeFilterValue rageValue)
        {
            return (rageValue.Lower ?? "*") + "-" + (rageValue.Upper ?? "*");
        }
    }
}
