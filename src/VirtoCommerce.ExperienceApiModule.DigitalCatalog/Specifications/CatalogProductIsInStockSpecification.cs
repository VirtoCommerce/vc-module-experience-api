using System;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Specifications
{
    public class CatalogProductIsInStockSpecification
    {
        public virtual bool IsSatisfiedBy(ExpProduct expProduct)
        {
            var result = AbstractTypeFactory<CatalogProductIsBuyableSpecification>.TryCreateInstance().IsSatisfiedBy(expProduct);

            if (!expProduct.IndexedProduct.TrackInventory.GetValueOrDefault(false))
            {
                return result;
            }

            return !expProduct.AllInventories.IsNullOrEmpty() &&
                (expProduct.AllInventories.Any(x => x.AllowBackorder)
                || expProduct.AllInventories.Any(x => x.AllowPreorder)
                || expProduct.AllInventories.Sum(inventory => Math.Max(0, inventory.InStockQuantity - inventory.ReservedQuantity)) > 0);
        }
    }
}
