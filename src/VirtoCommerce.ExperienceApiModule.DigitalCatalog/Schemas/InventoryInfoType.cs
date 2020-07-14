using GraphQL.Types;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class InventoryInfoType : ObjectGraphType<InventoryInfo>
    {
        public InventoryInfoType()
        {
            Name = "InventoryInfo";
            Description = "";
            Field<LongGraphType>("inStockQuantity", resolve: context => context.Source.InStockQuantity);
            Field(d => d.FulfillmentCenterId);
            Field(d => d.FulfillmentCenterName);
            Field<BooleanGraphType>("allowPreorder", resolve: context => context.Source.AllowPreorder);
            Field<BooleanGraphType>("allowBackorder", resolve: context => context.Source.AllowBackorder);
            Field<DateTimeGraphType>("preorderAvailabilityDate", resolve: context => context.Source.PreorderAvailabilityDate);
            Field<DateTimeGraphType>("backorderAvailabilityDate", resolve: context => context.Source.BackorderAvailabilityDate);
        }
    }
}
