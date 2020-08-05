using System;
using System.Linq;
using VirtoCommerce.ExperienceApiModule.Core.Index;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class IncludeFieldsExtensions
    {
        public static bool HasPricingFields(this IHasIncludeFields hasIncludeFields)
            => hasIncludeFields.IncludeFields.Any(x => x.Contains("prices", StringComparison.OrdinalIgnoreCase));

        public static bool HasInventoryFields(this IHasIncludeFields hasIncludeFields)
            => hasIncludeFields.IncludeFields.Any(x => x.Contains("availabilityData", StringComparison.OrdinalIgnoreCase));
    }
}
