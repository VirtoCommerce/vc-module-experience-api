using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog
{
    public class ExpAvailabilityData
    {
        public bool IsBuyable { get; set; }
        public bool IsActive { get; set; }
        public bool TrackInventory { get; set; }

        public IEnumerable<InventoryInfo> InventoryAll { get; set; } = Enumerable.Empty<InventoryInfo>();

        public virtual long AvailableQuantity
            => InventoryAll.Sum(inventory
                => Math.Max(0, inventory.InStockQuantity - inventory.ReservedQuantity));

        public virtual bool AllowPreorder => InventoryAll.Any(x => x.AllowPreorder);

        public virtual bool AllowBackorder => InventoryAll.Any(x => x.AllowBackorder);
    }
}
