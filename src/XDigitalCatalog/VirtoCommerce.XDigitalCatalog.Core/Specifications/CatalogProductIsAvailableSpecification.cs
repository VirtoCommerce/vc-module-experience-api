using System;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Specifications
{
    public class CatalogProductIsAvailableSpecification
    {
        public virtual bool IsSatisfiedBy(ExpProduct expProduct)
        {
            var result = AbstractTypeFactory<CatalogProductIsBuyableSpecification>.TryCreateInstance().IsSatisfiedBy(expProduct);

            if (result && expProduct.IndexedProduct.TrackInventory.GetValueOrDefault(false))
            {
                return !expProduct.AllInventories.IsNullOrEmpty() &&
                    (expProduct.AllInventories.Any(x => x.AllowBackorder)
                    || expProduct.AllInventories.Any(x => x.AllowPreorder)
                    || expProduct.AllInventories.Sum(inventory => Math.Max(0, inventory.InStockQuantity - inventory.ReservedQuantity)) >= 1);
            }

            return result;
        }
    }
}
