using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartItemsBulkCommand : CartCommandBase, ICommand<BulkCartResult>
    {
        public IList<NewBulkCartItem> CartItems { get; set; } = new List<NewBulkCartItem>();
    }
}
