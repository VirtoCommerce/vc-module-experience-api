using GraphQL.Types;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class ShippingMethodType : ObjectGraphType<ShippingRate>
    {
        public ShippingMethodType()
        {
            Field(x => x.ShippingMethod.Code, nullable: true).Description("Value of shipping gateway code");
            Field(x => x.ShippingMethod.LogoUrl, nullable: true).Description("Value of shipping method logo absolute URL");
            Field(x => x.OptionName, nullable: true).Description("Value of shipping method option name");
            Field(x => x.OptionDescription, nullable: true).Description("Value of shipping method option description");
            Field(x => x.ShippingMethod.Priority, nullable: true).Description("Value of shipping method priority");
            Field<CurrencyType>("currency", resolve: context => context.Source.Currency);
            Field<MoneyType>("price", resolve: context => context.Source.Rate);
            Field<MoneyType>("priceWithTax", resolve: context => context.Source.RateWithTax);
            Field<MoneyType>("total", resolve: context => context.Source.Rate - context.Source.DiscountAmount);
            Field<MoneyType>("totalWithTax", resolve: context => context.Source.RateWithTax - context.Source.DiscountAmountWithTax);
            Field<MoneyType>("discountAmount", resolve: context => context.Source.DiscountAmount);
            Field<MoneyType>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax);
            //TODO:
            //Field<MoneyType>("taxTotal", resolve: context => context.Source.TaxTotal);
            //Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            //Field(x => x.TaxType, nullable: true).Description("Tax type");
            //Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
            //Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
        }
    }
}
