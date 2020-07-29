using System;

namespace VirtoCommerce.XDigitalCatalog.Specifications
{
    public class CatalogProductIsBuyableSpecification
    {
        public virtual bool IsSatisfiedBy(ExpProduct product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return product.CatalogProduct.IsActive.GetValueOrDefault(false) && product.CatalogProduct.IsBuyable.GetValueOrDefault(false);
        }
    }
}
