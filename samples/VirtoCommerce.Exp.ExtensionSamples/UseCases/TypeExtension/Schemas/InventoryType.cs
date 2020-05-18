using GraphQL.Types;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class InventoryType : ObjectGraphType<Inventory>
    {
        public InventoryType()
        {
            Field(d => d.FulfillmentCenterId, nullable: true).Description("The warehouse id");
            Field(d => d.InStockQuantity, nullable: true).Description("in stock qty");
           
        }
     
    }
}
