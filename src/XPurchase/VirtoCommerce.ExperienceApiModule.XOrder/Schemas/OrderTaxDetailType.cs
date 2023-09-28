using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderTaxDetailType : ObjectGraphType<TaxDetail>
    {
        public OrderTaxDetailType()
        {
            Field<NonNullGraphType<MoneyType>>(nameof(TaxDetail.Rate).ToCamelCase(), resolve: context => new Money(context.Source.Rate, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(TaxDetail.Amount).ToCamelCase(), resolve: context => new Money(context.Source.Amount, context.GetOrderCurrency()));
            Field(x => x.Name);
        }
    }
}
