using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class AddCartItemCommand : CartCommand
    {
        public AddCartItemCommand()
        {
        }

        public AddCartItemCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string productId, int quantity, decimal? price, string comment)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
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
        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
