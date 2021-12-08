using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public abstract class WishlistCommand : ICommand<CartAggregate>
    {
        public string ListId { get; set; }
    }
}
