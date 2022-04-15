using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class BulkWishlistType : ExtendableGraphType<BulkCartAggregateResult>
    {
        public BulkWishlistType()
        {
            ExtendableField<ListGraphType<WishlistType>>("wishlists", "Wishlists", resolve: context => context.Source.CartAggregates);
        }
    }
}
