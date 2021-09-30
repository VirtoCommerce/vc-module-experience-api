using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputCreateOrderFromCartType : InputObjectGraphType
    {
        public InputCreateOrderFromCartType()
        {
            Field<StringGraphType>("cartId");
        }
    }
}
