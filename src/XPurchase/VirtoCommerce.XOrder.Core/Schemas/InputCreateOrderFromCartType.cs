using GraphQL.Types;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputCreateOrderFromCartType : InputObjectGraphType
    {
        public InputCreateOrderFromCartType()
        {
            Field<StringGraphType>("cartId",
                "Cart ID");
        }
    }
}
