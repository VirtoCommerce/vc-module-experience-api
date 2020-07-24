using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputCreateOrderFromCartType : InputObjectGraphType<StringGraphType>
    {
        public InputCreateOrderFromCartType()
        {
            Field<StringGraphType>("cartId");
        }
    }
}
