using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class PaymentType : ObjectGraphType<Payment>
    {
        public PaymentType()
        {
            Field(x => x.OuterId, nullable: false).Description("Value of payment outer id");
            Field(x => x.PaymentGatewayCode, nullable: false).Description("Value of payment gateway code");
            Field<CurrencyType>("currency", resolve: context => context.Source.Currency);
            Field<MoneyType>("amount", resolve: context => context.Source.Amount);
            Field<AddressType>("billingAddress", resolve: context => context.Source.BillingAddress);
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
