using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputShipmentType : InputObjectGraphType<ExpCartShipment>
    {
        public InputShipmentType()
        {
            Field(x => x.Id, nullable: true).Description("Shipment ID");
            Field(x => x.FulfillmentCenterId, nullable: true).Description("Fulfillment center iD");
            Field(x => x.Height, nullable: true).Description("Height value");
            Field(x => x.Length, nullable: true).Description("Length value");
            Field(x => x.MeasureUnit, nullable: true).Description("Measurement unit value");
            Field(x => x.ShipmentMethodCode, nullable: true).Description("Shipping method code");
            Field(x => x.ShipmentMethodOption, nullable: true).Description("Shipping method option");
            Field(x => x.VolumetricWeight, nullable: true).Description("Volumetric weight value");
            Field(x => x.Weight, nullable: true).Description("Weight value");
            Field(x => x.WeightUnit, nullable: true).Description("Weight unit value");
            Field(x => x.Width, nullable: true).Description("Width value");
            Field<InputAddressType>("deliveryAddress",
                "Delivery address");
            Field(x => x.Currency, nullable: true).Description("Currency value");
            Field(x => x.Price, nullable: true).Description("Price value");
            Field(x => x.VendorId, nullable: true).Description("Vendor ID");
            Field(x => x.Comment, nullable: true).Description("Text comment");
            //Field(x=> x.Items);

            Field<ListGraphType<InputDynamicPropertyValueType>>("dynamicProperties",
                "Dynamic properties");
        }
    }
}
