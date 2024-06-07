using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool ContainsAny(this IEnumerable<string> selector, params string[] values) => selector.Any(GetContainComparator(values));

        private static Func<string, bool> GetContainComparator(params string[] values) => x => values.Any()
            && values.Aggregate(false, (curr, acc) => curr || x.Contains(acc, StringComparison.OrdinalIgnoreCase));
    }
}
