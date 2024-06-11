using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Schemas
{
    public class AvailabilityDataType : ExtendableGraphType<ExpAvailabilityData>
    {
        public AvailabilityDataType()
        {
            Name = "AvailabilityData";

            Field(x => x.AvailableQuantity, nullable: false).Description("Available quantity");
            Field<NonNullGraphType<BooleanGraphType>>("IsBuyable",
                "Is buyable",
                resolve: context => context.Source.IsBuyable);
            Field<NonNullGraphType<BooleanGraphType>>("IsAvailable",
                "Is available",
                resolve: context => context.Source.IsAvailable);
            Field<NonNullGraphType<BooleanGraphType>>("IsInStock",
                "Is in stock",
                resolve: context => context.Source.IsInStock);
            Field<NonNullGraphType<BooleanGraphType>>("IsActive",
                "Is active",
                resolve: context => context.Source.IsActive);
            Field<NonNullGraphType<BooleanGraphType>>("IsTrackInventory",
                "Is track inventory",
                resolve: context => context.Source.IsTrackInventory);
            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<InventoryInfoType>>>>("inventories",
                "Inventories",
                resolve: context => context.Source.InventoryAll);
            Field<NonNullGraphType<BooleanGraphType>>("IsEstimated",
               "Is estimated",
               resolve: context => context.Source.IsEstimated);
        }
    }
}
