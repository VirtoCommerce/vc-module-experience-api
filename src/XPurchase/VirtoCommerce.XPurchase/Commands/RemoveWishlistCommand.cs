using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveWishlistCommand : ICommand<bool>
    {
        public string ListId { get; set; }

        public RemoveWishlistCommand(string listId)
        {
            ListId = listId;
        }
    }
}
