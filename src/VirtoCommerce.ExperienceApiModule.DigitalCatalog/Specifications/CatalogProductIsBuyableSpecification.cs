using System;
using System.Linq;

namespace VirtoCommerce.XDigitalCatalog.Specifications
{
    public class CatalogProductIsBuyableSpecification
    {
        public virtual bool IsSatisfiedBy(ExpProduct product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return product.IndexedProduct.IsActive.GetValueOrDefault(false)
                && product.IndexedProduct.IsBuyable.GetValueOrDefault(false)
                && (product.AllPrices.FirstOrDefault()?.ListPrice.Amount ?? 0) > 0;
        }
    }
}
