using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputShipmentType : InputObjectGraphType<Shipment>
    {
        public InputShipmentType()
        {
            //TODO: Need to load somehow from CartAggregate
            //Field(x => x.IsValid, nullable: true).Description("Is valid");
            //Field<ListGraphType<ValidationErrorType>>("validationErrors", resolve: context => context.Source.ValidationErrors);

            Field(x => x.FulfillmentCenterId, nullable: true).Description("Fulfillment center id");
            Field(x => x.Height, nullable: true).Description("Value of height");
            Field(x => x.Length, nullable: true).Description("Value of length");
            Field(x => x.MeasureUnit, nullable: true).Description("Value of measurement units");
            Field(x => x.ShipmentMethodCode, nullable: true).Description("Shipment method code");
            Field(x => x.ShipmentMethodOption, nullable: true).Description("Shipment method option");
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Tax type");
            Field(x => x.VolumetricWeight, nullable: true).Description("Value of volumetric weight");
            Field(x => x.Weight, nullable: true).Description("Value of weight");
            Field(x => x.WeightUnit, nullable: true).Description("Value of weight unit");
            Field(x => x.Width, nullable: true).Description("Value of width");
            Field<InputAddressType>("deliveryAddress", resolve: context => context.Source.DeliveryAddress);
            Field<InputCurrencyType>("currency", resolve: context => context.Source.Currency);
            Field<InputMoneyType>("discountAmount", resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("price", resolve: context => context.Source.Price.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("priceWithTax", resolve: context => context.Source.PriceWithTax.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("taxTotal", resolve: context => context.Source.TaxTotal.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("total", resolve: context => context.Source.Total.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("totalWithTax", resolve: context => context.Source.TotalWithTax.ToMoney(context.GetCart().Currency));
            Field<ListGraphType<InputCartShipmentItemType>>("items", resolve: context => context.Source.Items);
            Field<ListGraphType<InputDiscountType>>("discounts", resolve: context => context.Source.Discounts);
            Field<ListGraphType<InputTaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
        }
    }
}
