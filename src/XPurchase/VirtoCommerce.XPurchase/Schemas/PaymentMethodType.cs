using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class PaymentMethodType : ObjectGraphType<PaymentMethod>
    {
        public PaymentMethodType()
        {
            Field(x => x.Code, nullable: true).Description("Value of payment gateway code");
            //Field(x => x.Name, nullable: true).Description("Value of payment method name");
            Field(x => x.LogoUrl, nullable: true).Description("Value of payment method logo absolute URL");
            //Field(x => x.Description, nullable: true).Description("Value of payment method description");
            Field<StringGraphType>("paymentMethodType", description: "Value of payment method type", resolve: context => context.Source.PaymentMethodType.ToString());
            Field<StringGraphType>("paymentMethodGroupType", description: "Value of payment group type", resolve: context => context.Source.PaymentMethodGroupType.ToString());
            Field(x => x.Priority, nullable: true).Description("Value of payment method priority");
            Field(x => x.IsAvailableForPartial, nullable: true).Description("Is payment method available for partial payments");
            //Field<ListGraphType<SettingType>>("settings", resolve: context => context.Source.Settings);
            Field<CurrencyType>("currency",
                "Currency",
                resolve: context => context.GetCart().Currency);
            Field<MoneyType>("price",
                "Price",
                resolve: context => context.Source.Price.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("priceWithTax",
                "Price with tax",
                resolve: context => context.Source.PriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("total",
                "Total",
                resolve: context => context.Source.Total.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("totalWithTax",
                "Total with tax",
                resolve: context => context.Source.TotalWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountAmount",
                "Discount amount",
                resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountAmountWithTax",
                "Discount amount with tax",
                resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("taxTotal",
                "Tax total",
                resolve: context => context.Source.TaxTotal.ToMoney(context.GetCart().Currency));
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails",
                "Tax details",
                resolve: context => context.Source.TaxDetails);
            Field(x => x.Description, nullable: true).Description("Payment method description");
            //PT-5441: Extend the paymentmethod domain model
            //Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
        }
    }
}
