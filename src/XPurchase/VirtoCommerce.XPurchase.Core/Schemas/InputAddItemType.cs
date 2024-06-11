using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputAddItemType : InputCartBaseType
    {
        public InputAddItemType()
        {
            Field<NonNullGraphType<StringGraphType>>("productId",
                "Product ID");
            Field<NonNullGraphType<IntGraphType>>("quantity",
                "Quantity");
            Field<DecimalGraphType>("price",
                "Price");
            Field<StringGraphType>("comment",
                "Comment");

            Field<ListGraphType<InputDynamicPropertyValueType>>("dynamicProperties");
        }
    }
}
