using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class AvailabilityDataType : ExtendableGraphType<ExpAvailabilityData>
    {
        public AvailabilityDataType()
        {
            Name = "AvailabilityData";

            Field(x => x.AvailableQuantity, nullable: false).Description("Available quantity");
            Field<BooleanGraphType>("IsBuyable",
                "Is buyable",
                resolve: context => context.Source.IsBuyable);
            Field<BooleanGraphType>("IsAvailable",
                "Is available",
                resolve: context => context.Source.IsAvailable);
            Field<BooleanGraphType>("IsInStock",
                "Is in stock",
                resolve: context => context.Source.IsInStock);
            Field<BooleanGraphType>("IsActive",
                "Is active",
                resolve: context => context.Source.IsActive);
            Field<BooleanGraphType>("IsTrackInventory",
                "Is track inventory",
                resolve: context => context.Source.IsTrackInventory);
            ExtendableField<ListGraphType<InventoryInfoType>>("inventories",
                "Inventories",
                resolve: context => context.Source.InventoryAll);
        }
    }
}
