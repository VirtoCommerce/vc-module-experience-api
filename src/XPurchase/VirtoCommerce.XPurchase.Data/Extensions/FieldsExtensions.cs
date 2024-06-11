using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VirtoCommerce.XPurchase.Data.Extensions
{
    public static partial class FieldsExtensions
    {
        public static IList<string> ItemsToProductIncludeField(this IList<string> includeFields)
        {
            if (includeFields == null)
            {
                return null;
            }

            var result = includeFields
                    .Where(x => ProductFields().Match(x).Success)
                    .Select(x => ProductFields().Replace(x, string.Empty)).ToList();

            return result;
        }

        [GeneratedRegex(@"^(items\.)?items\.product\.")]
        private static partial Regex ProductFields();
    }
}
