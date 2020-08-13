using System;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Specifications
{
    public class CatalogProductIsInStockSpecification
    {
        public virtual bool IsSatisfiedBy(ExpProduct expProduct)
        {
            var result = new CatalogProductIsBuyableSpecification().IsSatisfiedBy(expProduct);

            if (!expProduct.IndexedProduct.TrackInventory.GetValueOrDefault(false) || expProduct.AllInventories.IsNullOrEmpty())
            {
                return result;
            }

            return expProduct.AllInventories.Any(x => x.AllowBackorder)
                || expProduct.AllInventories.Any(x => x.AllowPreorder)
                || expProduct.AllInventories.Sum(inventory => Math.Max(0, inventory.InStockQuantity - inventory.ReservedQuantity)) > 0;
        }
    }
}
