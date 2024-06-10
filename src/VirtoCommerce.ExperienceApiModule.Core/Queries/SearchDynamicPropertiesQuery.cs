using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SearchDynamicPropertiesQuery : IDynamicPropertiesQuery, IQuery<SearchDynamicPropertiesResponse>
    {
        public string CultureName { get; set; }
        public string Filter { get; set; }
        public string ObjectType { get; set; }
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
