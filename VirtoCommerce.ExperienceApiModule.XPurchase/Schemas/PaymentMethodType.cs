using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class PaymentMethodType : ObjectGraphType<PaymentMethod>
    {
        public PaymentMethodType()
        {
            Field(x => x.Code, nullable: true).Description("value of payment gateway code");
            Field(x => x.Name, nullable: true).Description("value of payment method name");
            Field(x => x.LogoUrl, nullable: true).Description("value of payment method logo absolute URL");
            Field(x => x.Description, nullable: true).Description("value of payment method description");
            Field(x => x.PaymentMethodType, nullable: true).Description("value of payment method type");
            Field(x => x.PaymentMethodGroupType, nullable: true).Description("value of payment group type");
            Field(x => x.Priority, nullable: true).Description("value of payment method priority");
            Field(x => x.IsAvailableForPartial, nullable: true).Description("Is payment method available for partial payments");
            Field<ListGraphType<SettingType>>("settings", resolve: context => context.Source.Settings);
            Field<ObjectGraphType<CurrencyType>>("currency", resolve: context => context.Source.Currency);
            Field<ObjectGraphType<MoneyType>>("price", resolve: context => context.Source.Price);
            Field<ObjectGraphType<MoneyType>>("priceWithTax", resolve: context => context.Source.PriceWithTax);
            Field<ObjectGraphType<MoneyType>>("total", resolve: context => context.Source.Total);
            Field<ObjectGraphType<MoneyType>>("totalWithTax", resolve: context => context.Source.TotalWithTax);
            Field<ObjectGraphType<MoneyType>>("discountAmount", resolve: context => context.Source.DiscountAmount);
            Field<ObjectGraphType<MoneyType>>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax);
            Field<ObjectGraphType<MoneyType>>("taxTotal", resolve: context => context.Source.TaxTotal);
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
            Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
        }
    }
}
