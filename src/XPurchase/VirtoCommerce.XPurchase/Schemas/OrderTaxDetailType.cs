using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class OrderTaxDetailType : ObjectGraphType<TaxDetail>
    {
        public OrderTaxDetailType()
        {
            Field<MoneyType>("amount", resolve: context => context.Source.Amount.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("price", resolve: context => context.Source.Rate.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("rate", resolve: context => context.Source.Rate.ToMoney(context.GetCart().Currency));
            Field<StringGraphType>("name", resolve: context => context.Source.Name);
        }
    }
}
