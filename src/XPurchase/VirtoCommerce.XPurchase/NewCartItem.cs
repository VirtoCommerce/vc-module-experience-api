using System.Collections.Generic;

namespace VirtoCommerce.XPurchase
{
    public class NewCartItem
    {
        public NewCartItem(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
        public string ProductId { get;  private set; }

        public CartProduct CartProduct { get; set; }

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
