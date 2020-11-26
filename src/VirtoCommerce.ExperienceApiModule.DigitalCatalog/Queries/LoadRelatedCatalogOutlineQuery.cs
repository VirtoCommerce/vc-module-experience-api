using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Outlines;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadRelatedCatalogOutlineQuery : CatalogQueryBase<LoadRelatedCatalogOutlineResponse>
    {
        public IList<Outline> Outlines { get; set; }
    }
}
