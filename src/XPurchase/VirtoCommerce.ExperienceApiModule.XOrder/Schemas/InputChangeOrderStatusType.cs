using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputChangeOrderStatusType : InputObjectGraphType
    {
        public InputChangeOrderStatusType()
        {
            Field<NonNullGraphType<StringGraphType>>("orderId",
                "Order ID");
            Field<NonNullGraphType<StringGraphType>>("status",
                "Order status");
        }
    }
}
