using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductResponse
    {
        public ICollection<ExpProduct> Products { get; set; } = new List<ExpProduct>();
    }
}
