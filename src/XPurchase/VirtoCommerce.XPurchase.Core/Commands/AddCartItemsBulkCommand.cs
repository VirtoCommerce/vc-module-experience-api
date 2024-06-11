using System.Collections.Generic;
using MediatR;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class AddCartItemsBulkCommand : IRequest<BulkCartResult>
    {
        public string CartId { get; set; }
        public string StoreId { get; set; }
        public string CartType { get; set; }
        public string CartName { get; set; } = "default";
        public string UserId { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureName { get; set; }

        public IList<NewBulkCartItem> CartItems { get; set; } = new List<NewBulkCartItem>();
    }
}
