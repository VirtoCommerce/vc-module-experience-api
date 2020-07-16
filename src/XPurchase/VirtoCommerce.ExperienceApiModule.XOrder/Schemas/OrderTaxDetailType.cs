using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderTaxDetailType : ObjectGraphType<TaxDetail>
    {
        public OrderTaxDetailType()
        {
            Field<OrderMoneyType>(nameof(TaxDetail.Rate).ToCamelCase(), resolve: context => new Money(context.Source.Rate, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(TaxDetail.Amount).ToCamelCase(), resolve: context => new Money(context.Source.Amount, context.GetOrder().Currency));
            Field(x => x.Name);
        }
    }
}
