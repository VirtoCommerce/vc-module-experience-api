using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class BulkWishlistType : ExtendableGraphType<BulkCartAggregateResult>
    {
        public BulkWishlistType()
        {
            ExtendableField<ListGraphType<WishlistType>>("wishlists", "Wishlists", resolve: context => context.Source.CartAggregates);
        }
    }
}
