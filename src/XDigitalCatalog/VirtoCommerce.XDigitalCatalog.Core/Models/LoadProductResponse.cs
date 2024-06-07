using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Core.Models
{
    public class LoadProductResponse
    {
        public LoadProductResponse(ICollection<ExpProduct> expProducts)
        {
            Products = expProducts;
        }

        public ICollection<ExpProduct> Products { get; private set; }
    }
}
