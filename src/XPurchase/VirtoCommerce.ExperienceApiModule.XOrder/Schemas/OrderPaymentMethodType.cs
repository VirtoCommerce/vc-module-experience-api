using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderPaymentMethodType : ObjectGraphType<PaymentMethod>
    {
        public OrderPaymentMethodType()
        {
            Field(x => x.Code, nullable: false);
            Field(x => x.Name, nullable: true);
            Field(x => x.Description, nullable: true);
            Field(x => x.LogoUrl, nullable: true);
            Field(x => x.Priority, nullable: false);
            Field(x => x.IsActive, nullable: false);
            Field(x => x.IsAvailableForPartial, nullable: false);
            Field(x => x.TypeName, nullable: false);
            Field(x => x.StoreId, nullable: true);

            Field<NonNullGraphType<CurrencyType>>(nameof(PaymentMethod.Currency).ToCamelCase(),
                resolve: context => context.GetOrderCurrency());

            Field<NonNullGraphType<MoneyType>>(nameof(PaymentMethod.Price).ToCamelCase(),
                resolve: context => new Money(context.Source.Price, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(PaymentMethod.PriceWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.PriceWithTax, context.GetOrderCurrency()));

            Field<NonNullGraphType<MoneyType>>(nameof(PaymentMethod.DiscountAmount).ToCamelCase(),
                resolve: context => new Money(context.Source.DiscountAmount, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(PaymentMethod.DiscountAmountWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.DiscountAmountWithTax, context.GetOrderCurrency()));

            Field<NonNullGraphType<MoneyType>>(nameof(PaymentMethod.Total).ToCamelCase(),
                resolve: context => new Money(context.Source.Total, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(PaymentMethod.TotalWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.TotalWithTax, context.GetOrderCurrency()));

            Field(x => x.TaxType, nullable: true);
            Field(x => x.TaxPercentRate, nullable: false);
            Field<NonNullGraphType<MoneyType>>(nameof(PaymentMethod.TaxTotal).ToCamelCase(),
                resolve: context => new Money(context.Source.TaxTotal, context.GetOrderCurrency()));
            Field<ListGraphType<NonNullGraphType<OrderTaxDetailType>>>(nameof(PaymentMethod.TaxDetails),
                resolve: x => x.Source.TaxDetails);

            Field<NonNullGraphType<PaymentMethodTypeType>>(nameof(PaymentMethod.PaymentMethodType),
                resolve: context => context.Source.PaymentMethodType);
            Field<NonNullGraphType<PaymentMethodGroupTypeType>>(nameof(PaymentMethod.PaymentMethodGroupType),
                resolve: context => context.Source.PaymentMethodGroupType);

            //PT-5383: Add additional properties to XOrder types:
            //public ICollection<ObjectSettingEntry> Settings);
        }
    }
}
