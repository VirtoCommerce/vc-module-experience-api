using System;

namespace VirtoCommerce.XPurchase
{
    public class ProductIsBuyableSpecification
    {
        public virtual bool IsSatisfiedBy(CartProduct product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return product.Product.IsActive.GetValueOrDefault(false) && product.Product.IsBuyable.GetValueOrDefault(false);
        }

    }
}
