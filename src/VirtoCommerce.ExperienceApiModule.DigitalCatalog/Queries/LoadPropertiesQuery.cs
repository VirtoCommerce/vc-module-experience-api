using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadPropertiesQuery : IQuery<LoadPropertiesResponse>
    {
        public IEnumerable<string> Ids { get; set; }
    }
}
