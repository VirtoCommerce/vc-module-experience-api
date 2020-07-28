using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Specifications
{
    public class CatalogProductIsAvailableSpecification
    {
        public virtual bool IsSatisfiedBy(CatalogProduct catalogProduct, IEnumerable<InventoryInfo> InventoryAll)
        {
            var result = new CatalogProductIsBuyableSpecification().IsSatisfiedBy(catalogProduct);

            if (result && catalogProduct.TrackInventory.GetValueOrDefault(false) && !InventoryAll.IsNullOrEmpty())
            {
                return InventoryAll.Any(x => x.AllowBackorder)
                    || InventoryAll.Any(x => x.AllowPreorder)
                    || InventoryAll.Sum(inventory => Math.Max(0, inventory.InStockQuantity - inventory.ReservedQuantity)) >= 1;
            }

            return result;
        }
    }
}
