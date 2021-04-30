using System.Collections.Generic;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SearchDynamicPropertiesResponse
    {
        public IList<DynamicProperty> Results { get; set; }
        public int TotalCount { get; set; }
    }
}
