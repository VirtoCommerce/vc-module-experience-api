using System;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog
{
    public class ProductIsBuyableSpecification
    {
        public virtual bool IsSatisfiedBy(ExpProduct product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return (product.CatalogProduct.IsActive ?? true) && (product.CatalogProduct.IsBuyable ?? true);
        }

    }
}
