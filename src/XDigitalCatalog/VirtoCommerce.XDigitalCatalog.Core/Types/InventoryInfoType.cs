using GraphQL.Types;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Core.Types
{
    public class InventoryInfoType : ObjectGraphType<InventoryInfo>
    {
        public InventoryInfoType()
        {
            Name = "InventoryInfo";
            Description = "";
            Field<NonNullGraphType<LongGraphType>>("inStockQuantity",
                "Inventory in stock quantity",
                resolve: context => context.Source.InStockQuantity);
            Field<NonNullGraphType<LongGraphType>>("reservedQuantity",
                "Inventory reserved quantity",
                resolve: context => context.Source.ReservedQuantity);
            Field(d => d.FulfillmentCenterId);
            Field(d => d.FulfillmentCenterName);
            Field<NonNullGraphType<BooleanGraphType>>("allowPreorder",
                "Allow preorder",
                resolve: context => context.Source.AllowPreorder);
            Field<NonNullGraphType<BooleanGraphType>>("allowBackorder",
                "Allow backorder",
                resolve: context => context.Source.AllowBackorder);
            Field<DateTimeGraphType>("preorderAvailabilityDate",
                "Preorder availability date",
                resolve: context => context.Source.PreorderAvailabilityDate);
            Field<DateTimeGraphType>("backorderAvailabilityDate",
                "Backorder availability date",
                resolve: context => context.Source.BackorderAvailabilityDate);
        }
    }
}
