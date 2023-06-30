using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductsQuery : CatalogQueryBase<LoadProductResponse>
    {
        public IList<string> ObjectIds { get; set; }
        public bool EvaluatePromotions { get; set; } = true;
    }
}
