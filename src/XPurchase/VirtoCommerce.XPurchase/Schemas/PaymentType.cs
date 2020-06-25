using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class PaymentType : ObjectGraphType<Payment>
    {
        public PaymentType()
        {
            Field(x => x.OuterId, nullable: false).Description("Value of payment outer id");
            Field(x => x.PaymentGatewayCode, nullable: false).Description("Value of payment gateway code");
            Field<CurrencyType>("currency", resolve: context => context.Source.Currency);
            Field<MoneyType>("amount", resolve: context => context.Source.Amount.ToMoney(context.Source.Currency));
            Field<AddressType>("billingAddress", resolve: context => context.Source.BillingAddress);
            Field<MoneyType>("price", resolve: context => context.Source.Price.ToMoney(context.Source.Currency));
            Field<MoneyType>("priceWithTax", resolve: context => context.Source.PriceWithTax.ToMoney(context.Source.Currency));
            Field<MoneyType>("total", resolve: context => context.Source.Total.ToMoney(context.Source.Currency));
            Field<MoneyType>("totalWithTax", resolve: context => context.Source.TotalWithTax.ToMoney(context.Source.Currency));
            Field<MoneyType>("discountAmount", resolve: context => context.Source.DiscountAmount.ToMoney(context.Source.Currency));
            Field<MoneyType>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.Source.Currency));
            Field<MoneyType>("taxTotal", resolve: context => context.Source.TaxTotal.ToMoney(context.Source.Currency));
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
            Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
        }
    }
}
