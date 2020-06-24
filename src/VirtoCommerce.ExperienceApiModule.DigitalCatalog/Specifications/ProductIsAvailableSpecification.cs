using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog
{
    public class ProductIsAvailableSpecification
    {
        private readonly ExpProduct _product;
        public ProductIsAvailableSpecification(ExpProduct product)
        {
            _product = product;
        }

        public virtual bool IsSatisfiedBy(long requestedQuantity)
        {
            var result = new ProductIsBuyableSpecification().IsSatisfiedBy(_product);

            if (result && (_product.CatalogProduct.TrackInventory ?? false) && !_product.Inventories.IsNullOrEmpty())
            {
                //TODO: Need to use inventory info relevant to execution context and user
                var firstInventory = _product.Inventories.FirstOrDefault();
                result = firstInventory.AllowPreorder ||
                              firstInventory.AllowBackorder  ||
                              firstInventory.InStockQuantity >= requestedQuantity;
            }

            return result;
        }

    }
}
