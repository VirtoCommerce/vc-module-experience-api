using GraphQL.Types;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class PaymentMethodType : ObjectGraphType<PaymentMethod>
    {
        public PaymentMethodType()
        {
            Field(x => x.Code, nullable: true).Description("Value of payment gateway code");
            Field(x => x.Name, nullable: true).Description("Value of payment method name");
            Field(x => x.LogoUrl, nullable: true).Description("Value of payment method logo absolute URL");
            //Field(x => x.Description, nullable: true).Description("Value of payment method description");
            Field(x => x.PaymentMethodType, nullable: true).Description("Value of payment method type");
            Field(x => x.PaymentMethodGroupType, nullable: true).Description("Value of payment group type");
            Field(x => x.Priority, nullable: true).Description("Value of payment method priority");
            Field(x => x.IsAvailableForPartial, nullable: true).Description("Is payment method available for partial payments");
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
            //TODO: Extend the paymentmethod domain model
            //Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
        }
    }
}
