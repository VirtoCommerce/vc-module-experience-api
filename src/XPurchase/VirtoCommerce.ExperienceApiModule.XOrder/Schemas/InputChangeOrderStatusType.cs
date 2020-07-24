using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputChangeOrderStatusType : InputObjectGraphType
    {
        public InputChangeOrderStatusType()
        {
            Field<StringGraphType>("orderId");
            Field<StringGraphType>("status");
        }
    }
}
