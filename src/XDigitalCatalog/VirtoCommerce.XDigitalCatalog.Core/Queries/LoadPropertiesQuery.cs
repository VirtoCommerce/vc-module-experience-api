using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Queries
{
    public class LoadPropertiesQuery : IQuery<LoadPropertiesResponse>
    {
        public IEnumerable<string> Ids { get; set; }
    }
}
