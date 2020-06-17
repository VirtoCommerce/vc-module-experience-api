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
            Field<ObjectGraphType<CurrencyType>>("currency", resolve: context => context.Source.Currency);
            Field<ObjectGraphType<MoneyType>>("amount", resolve: context => context.Source.Amount);
            Field<ObjectGraphType<AddressType>>("billingAddress", resolve: context => context.Source.BillingAddress);
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
