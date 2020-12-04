using System;
using System.Collections.Generic;
using System.Text;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderPaymentMethodType : ObjectGraphType<PaymentMethod>
    {
        public OrderPaymentMethodType()
        {
            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(PaymentMethod.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field(x => x.TaxPercentRate);
            Field<MoneyType>(nameof(PaymentMethod.TaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.TaxTotal, context.GetOrderCurrency()));
            Field(x => x.TaxType, true);
            Field(x => x.TypeName);
            Field(x => x.StoreId);
            Field<MoneyType>(nameof(PaymentMethod.TotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.TotalWithTax, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(PaymentMethod.Total).ToCamelCase(), resolve: context => new Money(context.Source.Total, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(PaymentMethod.DiscountAmount).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmount, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(PaymentMethod.DiscountAmountWithTax).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmountWithTax, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(PaymentMethod.Price).ToCamelCase(), resolve: context => new Money(context.Source.Price, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(PaymentMethod.PriceWithTax).ToCamelCase(), resolve: context => new Money(context.Source.PriceWithTax, context.GetOrderCurrency()));
            Field<CurrencyType>(nameof(PaymentMethod.Currency).ToCamelCase(), resolve: context => context.GetOrderCurrency());          
            Field(x => x.IsAvailableForPartial);
            Field(x => x.Priority);
            Field(x => x.IsActive);
            Field(x => x.LogoUrl);
            Field(x => x.Code);
            Field<IntGraphType>(nameof(PaymentMethod.PaymentMethodType), resolve: context => (int)context.Source.PaymentMethodType);
            Field<IntGraphType>(nameof(PaymentMethod.PaymentMethodGroupType), resolve: context => (int)context.Source.PaymentMethodGroupType);

            //TODO
            //public ICollection<ObjectSettingEntry> Settings);
        }
    }
}
