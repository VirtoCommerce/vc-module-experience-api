using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Outlines;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadRelatedSlugPathQuery : CatalogQueryBase<LoadRelatedSlugPathResponse>
    {
        public IList<Outline> Outlines { get; set; }
    }
}
