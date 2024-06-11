using GraphQL;
using GraphQL.Types;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Core.Schemas
{
    public class FulfillmentCenterType : ObjectGraphType<FulfillmentCenter>
    {
        public FulfillmentCenterType(IFulfillmentCenterGeoService fulfillmentCenterGeoService)
        {
            Field(x => x.Id).Description("Fulfillment Center ID.");
            Field(x => x.Name, nullable: true).Description("Fulfillment Center name.");
            Field(x => x.Description, nullable: true).Description("Fulfillment Center descripion.");
            Field(x => x.OuterId, nullable: true).Description("Fulfillment Center outerId.");
            Field(x => x.GeoLocation, nullable: true).Description("Fulfillment Center geo location.");
            Field(x => x.ShortDescription, nullable: true).Description("Fulfillment Center short description.");
            Field<FulfillmentCenterAddressType>(nameof(FulfillmentCenter.Address).ToCamelCase(),
                description: "Fulfillment Center address.",
                resolve: x => x.Source.Address);

            FieldAsync<ListGraphType<FulfillmentCenterType>>(
                "nearest",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "take" }),
                description: "Nearest Fulfillment Centers",
                resolve: async context =>
                {
                    var take = context.GetArgument("take", 10);

                    var result = await fulfillmentCenterGeoService.GetNearestAsync(context.Source.Id, take);
                    return result;
                });
        }
    }
}
