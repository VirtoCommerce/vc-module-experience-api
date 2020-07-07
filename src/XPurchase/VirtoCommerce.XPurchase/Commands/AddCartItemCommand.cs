using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartItemCommand : CartCommand
    {
        public AddCartItemCommand()
        {
        }

        public AddCartItemCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string productId, int quantity, decimal? price, string comment)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            ProductId = productId;
            Quantity = quantity;
            Comment = comment;
            Price = price;
        }

        public string ProductId { get; set; }
        public int Quantity { get; set; }

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
