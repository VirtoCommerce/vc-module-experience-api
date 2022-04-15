using System.Collections.Generic;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddWishlistBulkItemCommand : IRequest<BulkCartAggregateResult>
    {
        public string ProductId { get; set; }

        public IList<string> ListIds { get; set; }
    }
}
