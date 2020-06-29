using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public abstract class InputCartBaseType : InputObjectGraphType
    {
        protected InputCartBaseType()
        {
            Field<NonNullGraphType<StringGraphType>>("storeId");
            Field<NonNullGraphType<StringGraphType>>("cartName");
            Field<NonNullGraphType<StringGraphType>>("userId");
            Field<NonNullGraphType<StringGraphType>>("currency");
            Field<NonNullGraphType<StringGraphType>>("language");
            Field<NonNullGraphType<StringGraphType>>("cartType");
        }
       
    }
}
