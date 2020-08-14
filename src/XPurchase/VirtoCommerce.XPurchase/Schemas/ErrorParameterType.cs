using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class ErrorParameterType : ObjectGraphType<ErrorParameter>
    {
        public ErrorParameterType()
        {
            Field(x=>x.Key).Description("key");
            Field(x => x.Value).Description("Value");
        }
    }
}
