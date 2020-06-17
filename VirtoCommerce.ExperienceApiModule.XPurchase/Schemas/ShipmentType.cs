using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class ShipmentType : ObjectGraphType<Shipment>
    {
        public ShipmentType()
        {
            Field(x => x.ShipmentMethodCode, nullable: true).Description("Shipment method code");
            Field(x => x.ShipmentMethodOption, nullable: true).Description("Shipment method option");
            Field(x => x.FulfillmentCenterId, nullable: true).Description("Fulfillment center id");
            Field<ObjectGraphType<AddressType>>("deliveryAddress", resolve: context => context.Source.DeliveryAddress);
            Field(x => x.VolumetricWeight, nullable: true).Description("Value of volumetric weight");
            Field(x => x.WeightUnit, nullable: true).Description("Value of weight unit");
            Field(x => x.Weight, nullable: true).Description("Value of weight");
            Field(x => x.MeasureUnit, nullable: true).Description("Value of measurement units");
            Field(x => x.Height, nullable: true).Description("Value of height");
            Field(x => x.Length, nullable: true).Description("Value of length");
            Field(x => x.Width, nullable: true).Description("Value of width");
            Field<ObjectGraphType<MoneyType>>("price", resolve: context => context.Source.Price);
            Field<ObjectGraphType<MoneyType>>("priceWithTax", resolve: context => context.Source.PriceWithTax);
            Field<ObjectGraphType<MoneyType>>("total", resolve: context => context.Source.Total);
            Field<ObjectGraphType<MoneyType>>("totalWithTax", resolve: context => context.Source.TotalWithTax);
            Field<ObjectGraphType<MoneyType>>("discountAmount", resolve: context => context.Source.DiscountAmount);
            Field<ObjectGraphType<MoneyType>>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax);
            Field<ListGraphType<CartShipmentItemType>>("items", resolve: context => context.Source.Items);
            Field<ObjectGraphType<MoneyType>>("taxTotal", resolve: context => context.Source.TaxTotal);
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
            Field(x => x.IsValid, nullable: true).Description("Is valid");
            Field<ListGraphType<ValidationErrorType>>("validationErrors", resolve: context => context.Source.ValidationErrors);
            Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
            Field<ObjectGraphType<CurrencyType>>("currency", resolve: context => context.Source.Currency);
        }
    }
}
