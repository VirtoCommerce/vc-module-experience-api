using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadPromotionsQuery : IQuery<LoadPromotionsResponce>
    {
        public IEnumerable<string> Ids { get; set; }
    }
}
