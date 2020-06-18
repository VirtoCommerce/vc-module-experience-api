using System;
using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Catalog
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
