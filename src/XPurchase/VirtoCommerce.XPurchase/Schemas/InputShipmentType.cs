using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputShipmentType : InputObjectGraphType<ExpCartShipment>
    {
        public InputShipmentType()
        {
            Field(x => x.Id, nullable: true).Description("Shipment Id");
            Field(x => x.FulfillmentCenterId, nullable: true).Description("Fulfillment center id");
            Field(x => x.Height, nullable: true).Description("Value of height");
            Field(x => x.Length, nullable: true).Description("Value of length");
            Field(x => x.MeasureUnit, nullable: true).Description("Value of measurement units");
            Field(x => x.ShipmentMethodCode, nullable: true).Description("Shipment method code");
            Field(x => x.ShipmentMethodOption, nullable: true).Description("Shipment method option");
            Field(x => x.VolumetricWeight, nullable: true).Description("Value of volumetric weight");
            Field(x => x.Weight, nullable: true).Description("Value of weight");
            Field(x => x.WeightUnit, nullable: true).Description("Value of weight unit");
            Field(x => x.Width, nullable: true).Description("Value of width");
            //TODO: Add descriptions
            Field<InputAddressType>("deliveryAddress");
            Field(x => x.Currency, nullable: true);
            Field(x => x.Price, nullable: true);
            //Field(x=> x.Items);

            Field<ListGraphType<InputDynamicPropertyValueType>>("dynamicProperties");
        }
    }
}
