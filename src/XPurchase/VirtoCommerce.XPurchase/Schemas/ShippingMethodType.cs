using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class ShippingMethodType : ObjectGraphType<ShippingRate>
    {
        public ShippingMethodType()
        {
            Field<NonNullGraphType<StringGraphType>>("id", resolve: context => string.Join("_", context.Source.ShippingMethod.Code, context.Source.OptionName));
            Field(x => x.ShippingMethod.Code, nullable: false).Description("Value of shipping gateway code");
            Field(x => x.ShippingMethod.LogoUrl, nullable: true).Description("Value of shipping method logo absolute URL");
            Field(x => x.ShippingMethod.Name, nullable: true).Description("Shipping method name");
            Field(x => x.ShippingMethod.Description, nullable: true).Description("Shipping method description");
            Field(x => x.OptionName, nullable: true).Description("Value of shipping method option name");
            Field(x => x.OptionDescription, nullable: true).Description("Value of shipping method option description");
            Field(x => x.ShippingMethod.Priority, nullable: false).Description("Value of shipping method priority");
            Field<NonNullGraphType<CurrencyType>>("currency",
                "Currency",
                resolve: context => context.GetCart().Currency);
            Field<NonNullGraphType<MoneyType>>("price",
                "Price",
                resolve: context => context.Source.Rate.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("priceWithTax",
                "Price with tax",
                resolve: context => context.Source.RateWithTax.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("total",
                "Total",
                resolve: context => (context.Source.Rate - context.Source.DiscountAmount).ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("totalWithTax",
                "Total with tax",
                resolve: context => (context.Source.RateWithTax - context.Source.DiscountAmountWithTax).ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("discountAmount",
                "Discount amount",
                resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("discountAmountWithTax",
                "Discount amount with tax",
                resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCart().Currency));
            //PT-5449: Add fields to ShippingMethodType
            //Field<MoneyType>("taxTotal", resolve: context => context.Source.TaxTotal);
            //Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            //Field(x => x.TaxType, nullable: true).Description("Tax type");
            //Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
            //Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
        }
    }
}
