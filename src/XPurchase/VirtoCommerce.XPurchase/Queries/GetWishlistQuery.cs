using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Queries
{
    public class GetWishlistQuery : IQuery<CartAggregate>
    {
        public string ListId { get; set; }
    }
}
