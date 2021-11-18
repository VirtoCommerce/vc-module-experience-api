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
            Field<MoneyType>("amount",
                "Amount",
                resolve: context => context.Source.Amount.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("price",
                "Price",
                resolve: context => context.Source.Rate.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("rate",
                "Rate",
                resolve: context => context.Source.Rate.ToMoney(context.GetCart().Currency));
            Field<StringGraphType>("name",
                "Name",
                resolve: context => context.Source.Name);
        }
    }
}
