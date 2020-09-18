using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public abstract class InputCartBaseType : InputObjectGraphType
    {
        protected InputCartBaseType()
        {
            Field<StringGraphType>("cartId");
            Field<NonNullGraphType<StringGraphType>>("storeId");
            Field<StringGraphType>("cartName");
            Field<NonNullGraphType<StringGraphType>>("userId");
            Field<StringGraphType>("currency");
            Field<StringGraphType>("language");
            Field<StringGraphType>("cartType");
        }
    }
}
