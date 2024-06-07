using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Queries
{
    public class LoadRelatedSlugPathQuery : CatalogQueryBase<LoadRelatedSlugPathResponse>
    {
        public IList<Outline> Outlines { get; set; }
    }
}
