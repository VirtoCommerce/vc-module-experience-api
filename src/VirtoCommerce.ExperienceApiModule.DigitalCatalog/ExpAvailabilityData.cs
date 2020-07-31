using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog
{
    public class ExpAvailabilityData
    {
        public bool IsBuyable { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsInStock { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<InventoryInfo> InventoryAll { get; set; } = Enumerable.Empty<InventoryInfo>();

        public long AvailableQuantity { get; set; }
    }
}
