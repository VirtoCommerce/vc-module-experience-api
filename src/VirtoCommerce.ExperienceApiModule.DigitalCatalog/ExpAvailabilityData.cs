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
        public bool IsTrackInventory { get; set; }
        /// <summary>
        /// This flag is used to indicate whether a offer  is estimated or represents an actual value.
        /// When set to true, it signifies that the product price and availability is an estimation,
        /// often used when unable to get actual price and availability information  or when the system is using cached offer information
        /// </summary>
        public bool IsEstimated { get; set; }
        public IEnumerable<InventoryInfo> InventoryAll { get; set; } = Enumerable.Empty<InventoryInfo>();
        public long AvailableQuantity { get; set; }
        public virtual ExpAvailabilityData FromProduct(ExpProduct product)
        {
            AvailableQuantity = product.AvailableQuantity;
            InventoryAll = product.AllInventories;
            IsBuyable = product.IsBuyable;
            IsAvailable = product.IsAvailable;
            IsInStock = product.IsInStock;
            IsActive = product.IndexedProduct?.IsActive ?? false;
            IsTrackInventory = product?.IndexedProduct?.TrackInventory ?? false;
            return this;
        }
    }
}
