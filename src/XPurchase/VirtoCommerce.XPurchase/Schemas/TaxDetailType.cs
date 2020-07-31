using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class TaxDetailType : ObjectGraphType<TaxDetail>
    {
        public TaxDetailType()
        {
            Field<MoneyType>("amount", resolve: context => context.Source.Amount.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("price", resolve: context => context.Source.Rate.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("rate", resolve: context => context.Source.Rate.ToMoney(context.GetCart().Currency));
            Field<StringGraphType>("name", resolve: context => context.Source.Name);
        }
    }
}
