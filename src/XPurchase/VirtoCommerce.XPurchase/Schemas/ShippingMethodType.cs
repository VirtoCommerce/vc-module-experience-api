using GraphQL.Types;
using VirtoCommerce.XPurchase.Models.Cart;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class ShippingMethodType : ObjectGraphType<ShippingMethod>
    {
        public ShippingMethodType()
        {
            Field(x => x.ShipmentMethodCode, nullable: true).Description("Value of shipping gateway code");
            Field(x => x.Name, nullable: true).Description("Value of shipping method name");
            Field(x => x.LogoUrl, nullable: true).Description("Value of shipping method logo absolute URL");
            Field(x => x.OptionName, nullable: true).Description("Value of shipping method option name");
            Field(x => x.OptionDescription, nullable: true).Description("Value of shipping method option description");
            Field(x => x.Priority, nullable: true).Description("Value of shipping method priority");
            Field<ListGraphType<SettingType>>("settings", resolve: context => context.Source.Settings);
            Field<CurrencyType>("currency", resolve: context => context.Source.Currency);
            Field<MoneyType>("price", resolve: context => context.Source.Price);
            Field<MoneyType>("priceWithTax", resolve: context => context.Source.PriceWithTax);
            Field<MoneyType>("total", resolve: context => context.Source.Total);
            Field<MoneyType>("totalWithTax", resolve: context => context.Source.TotalWithTax);
            Field<MoneyType>("discountAmount", resolve: context => context.Source.DiscountAmount);
            Field<MoneyType>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax);
            Field<MoneyType>("taxTotal", resolve: context => context.Source.TaxTotal);
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
            Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
        }
    }
}
