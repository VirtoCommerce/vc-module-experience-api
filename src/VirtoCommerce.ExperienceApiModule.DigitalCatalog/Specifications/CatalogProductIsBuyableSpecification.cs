using System;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Specifications
{
    public class CatalogProductIsBuyableSpecification
    {
        public virtual bool IsSatisfiedBy(CatalogProduct product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return product.IsActive.GetValueOrDefault(false) && product.IsBuyable.GetValueOrDefault(false);
        }
    }
}
