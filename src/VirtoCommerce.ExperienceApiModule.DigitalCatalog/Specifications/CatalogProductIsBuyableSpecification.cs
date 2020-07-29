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
            //TODO: Need to check if product has a price for requested currency
            return product.IndexedProduct.IsActive.GetValueOrDefault(false) && product.IndexedProduct.IsBuyable.GetValueOrDefault(false) && product.AllPrices.Any();
        }
    }
}
