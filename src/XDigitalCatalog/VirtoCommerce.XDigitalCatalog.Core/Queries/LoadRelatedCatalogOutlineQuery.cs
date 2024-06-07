using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Queries
{
    public class LoadRelatedCatalogOutlineQuery : CatalogQueryBase<LoadRelatedCatalogOutlineResponse>
    {
        public IList<Outline> Outlines { get; set; }
    }
}
