using System.Collections.Generic;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SearchDynamicPropertiesResponse
    {
        public IReadOnlyList<DynamicPropertyEntity> Results { get; set; }
        public int TotalCount { get; set; }
    }
}
