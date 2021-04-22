using System.Collections.Generic;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SearchDynamicPropertiesResponse
    {
        public IReadOnlyList<DynamicProperty> Results { get; set; }
        public int TotalCount { get; set; }
    }
}
