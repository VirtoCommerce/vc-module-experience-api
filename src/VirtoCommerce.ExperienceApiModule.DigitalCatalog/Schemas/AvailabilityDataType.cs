using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class AvailabilityDataType : ObjectGraphType<ExpAvailabilityData>
    {
        public AvailabilityDataType()
        {
            Name = "AvailabilityData";

            Field(x => x.AvailableQuantity, nullable: false).Description("Available quantity");

            Field<BooleanGraphType>("IsBuyable", resolve: context => context.Source.IsBuyable);

            Field<BooleanGraphType>("IsAvailable", resolve: context => context.Source.IsAvailable);

            Field<BooleanGraphType>("IsInStock", resolve: context => context.Source.IsInStock);

            Field<BooleanGraphType>("IsActive", resolve: context => context.Source.IsActive);

            Field<BooleanGraphType>("IsTrackInventory", resolve: context => context.Source.IsTrackInventory);

            Field(GraphTypeExtenstionHelper.GetComplexType<ListGraphType<InventoryInfoType>>(), "inventories", resolve: context => context.Source.InventoryAll);
        }
    }
}
