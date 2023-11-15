using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class WishlistResultType : ExtendableGraphType<BulkCartResult>
    {
        public WishlistResultType()
        {
            ExtendableField<CartType>("list", "Wishlst", resolve: context => context.Source.Cart);
            Field<ListGraphType<ValidationErrorType>>("errors", "Product errors", resolve: context => context.Source.Errors);
        }
    }
}
