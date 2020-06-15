using System;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog
{
    public class ProductIsBuyableSpecification : ISpecification<Product>
    {
        public virtual bool IsSatisfiedBy(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return product.IsActive && product.IsBuyable;
        }

    }
}
