using GraphQL.Types;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class AvailabilityDataType : ObjectGraphType<ExpAvailabilityData>
    {
        public AvailabilityDataType()
        {
            Name = "AvailabilityData";

            Field(x => x.AvailableQuantity, nullable: false).Description("Available quantity");

            Field<BooleanGraphType>("IsBuyable", resolve: context => context.Source.IsBuyable);

            Field<BooleanGraphType>("IsAvailable", resolve: context =>
            {
                var data = context.Source;

                var isAvailable = data.IsBuyable;

                if (isAvailable && data.TrackInventory && !data.InventoryAll.IsNullOrEmpty())
                {
                    return data.AllowBackorder
                        || data.AllowPreorder
                        || data.AvailableQuantity >= 1;
                }

                return isAvailable;
            });

            Field<BooleanGraphType>("IsInStock", resolve: context =>
            {
                var data = context.Source;

                if (!data.TrackInventory || data.InventoryAll.IsNullOrEmpty())
                {
                    return true;
                }

                return data.AllowBackorder
                    || data.AllowPreorder
                    || data.AvailableQuantity > 0;
            });

            Field<ListGraphType<InventoryInfoType>>("inventories", resolve: context => context.Source.InventoryAll);
        }
    }
}
