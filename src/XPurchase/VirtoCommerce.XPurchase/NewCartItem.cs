using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.XPurchase.Domain.CartAggregate
{
    public class NewCartItem
    {
        public NewCartItem(CatalogProduct product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
        public CatalogProduct Product { get;  private set; }

        public int Quantity { get; private set; }

        /// <summary>
        /// Manual price
        /// </summary>
        public decimal? Price { get; set; }


        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Dynamic properties
        /// </summary>
        public Dictionary<string, string> DynamicProperties { get; set; }
    }
}
