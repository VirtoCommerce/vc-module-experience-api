using System.Collections.Generic;
using VirtoCommerce.XDigitalCatalog;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductResponse
    {
        public ICollection<ExpProduct> Products { get; set; } = new List<ExpProduct>();
    }
}
