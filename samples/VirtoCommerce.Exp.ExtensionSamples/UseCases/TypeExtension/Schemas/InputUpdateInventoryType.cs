using GraphQL.Types;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class InputUpdateInventoryType : InputObjectGraphType
    {
        public InputUpdateInventoryType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(Inventory.ProductId));
            Field<NonNullGraphType<IntGraphType>>(nameof(Inventory.InStockQuantity));
        }
     
    }
}
