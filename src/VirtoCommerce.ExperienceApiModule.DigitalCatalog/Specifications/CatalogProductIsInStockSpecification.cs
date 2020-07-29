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

            if (!expProduct.CatalogProduct.TrackInventory.GetValueOrDefault(false) || expProduct.Inventories.IsNullOrEmpty())
            {
                return result;
            }

            return expProduct.Inventories.Any(x => x.AllowBackorder)
                || expProduct.Inventories.Any(x => x.AllowPreorder)
                || expProduct.Inventories.Sum(inventory => Math.Max(0, inventory.InStockQuantity - inventory.ReservedQuantity)) > 0;
        }
    }
}
