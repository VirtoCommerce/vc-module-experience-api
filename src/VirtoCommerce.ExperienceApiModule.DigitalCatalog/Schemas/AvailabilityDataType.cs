using GraphQL.Types;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class AvailabilityDataType : ObjectGraphType<ExpAvailabilityData>
    {
        public AvailabilityDataType()
        {
            Name = "AvailabilityData";

            Field(x => x.AvailableQuantity, nullable: false).Description("Available quantity");
            Field<ListGraphType<InventoryInfoType>>("inventories", resolve: context => context.Source.InventoryAll);
        }
    }
}
