using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Domain.Commands
{
    public class AddCartItemCommand : CartCommand
    {
        public AddCartItemCommand(string storeId, string cartType, string cartName, string userId, string currency, string language)
            :base(storeId, cartType, cartName, userId, currency, language)
        {
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
