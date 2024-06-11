using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class BulkCartType : ExtendableGraphType<BulkCartResult>
    {
        public BulkCartType()
        {
            ExtendableField<CartType>("cart",
                "Cart",
                resolve: context => context.Source.Cart);

            Field<ListGraphType<ValidationErrorType>>("errors",
                "A set of errors in case the Skus are invalid",
                resolve: context => context.Source.Errors);
        }
    }
}
