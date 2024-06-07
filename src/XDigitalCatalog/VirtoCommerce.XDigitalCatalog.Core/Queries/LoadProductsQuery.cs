using System.Collections.Generic;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Queries
{
    public class LoadProductsQuery : CatalogQueryBase<LoadProductResponse>
    {
        public IList<string> ObjectIds { get; set; }
        public bool EvaluatePromotions { get; set; } = true;
    }
}
