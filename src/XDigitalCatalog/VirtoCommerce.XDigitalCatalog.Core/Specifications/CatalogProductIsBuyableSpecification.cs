using System;
using System.Linq;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Specifications
{
    public class CatalogProductIsBuyableSpecification
    {
        /// <summary>
        /// Evaluates a product is buyable specification.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual bool IsSatisfiedBy(ExpProduct product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return product.IndexedProduct.IsActive.GetValueOrDefault(false)
                && product.IndexedProduct.IsBuyable.GetValueOrDefault(false)
                && CheckPricePolicy(product);
        }

        /// <summary>
        /// Represents a price policy for a product. By default, product price should be greater than zero.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        protected virtual bool CheckPricePolicy(ExpProduct product)
        {
            return (product.AllPrices.FirstOrDefault()?.ListPrice.Amount ?? 0) > 0;
        }
    }
}
