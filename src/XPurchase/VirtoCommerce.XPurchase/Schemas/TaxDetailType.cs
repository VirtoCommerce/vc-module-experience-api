using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.ExperienceApiModule.Core;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class TaxDetailType : ObjectGraphType<TaxDetail>
    {
        public TaxDetailType()
        {
            //TODO: Get currency from cart
            Field<MoneyType>("rate", resolve: context => context.Source.Rate.ToMoney("USD"));
            //TODO: Get currency from cart
            Field<MoneyType>("amount", resolve: context => context.Source.Amount.ToMoney("USD"));
            Field<StringGraphType>("name", resolve: context => context.Source.Name);
            //TODO: Get currency from cart
            Field<MoneyType>("price", resolve: context => context.Source.Rate.ToMoney("USD"));
        }
    }
}
