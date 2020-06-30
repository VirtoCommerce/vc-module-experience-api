using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputPaymentType : InputObjectGraphType<Payment>
    {
        public InputPaymentType()
        {
            Field(x => x.OuterId, nullable: false).Description("Value of payment outer id");
            Field(x => x.PaymentGatewayCode, nullable: false).Description("Value of payment gateway code");
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Tax type");
            Field<InputAddressType>("billingAddress", resolve: context => context.Source.BillingAddress);
            Field<InputCurrencyType>("currency", resolve: context => context.Source.Currency);
            Field<InputMoneyType>("amount", resolve: context => context.Source.Amount.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("discountAmount", resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("price", resolve: context => context.Source.Price.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("priceWithTax", resolve: context => context.Source.PriceWithTax.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("taxTotal", resolve: context => context.Source.TaxTotal.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("total", resolve: context => context.Source.Total.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("totalWithTax", resolve: context => context.Source.TotalWithTax.ToMoney(context.GetCart().Currency));
            Field<ListGraphType<InputDiscountType>>("discounts", resolve: context => context.Source.Discounts);
            Field<ListGraphType<InputTaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
        }
    }
}
