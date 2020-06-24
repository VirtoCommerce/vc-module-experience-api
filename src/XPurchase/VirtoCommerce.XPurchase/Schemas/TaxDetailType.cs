using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class TaxDetailType : ObjectGraphType<TaxDetail>
    {
        public TaxDetailType()
        {
            Field<MoneyType>("rate", resolve: context => context.Source.Rate);
            Field<MoneyType>("amount", resolve: context => context.Source.Amount);
            Field<MoneyType>("name", resolve: context => context.Source.Name);
            Field<MoneyType>("price", resolve: context => context.Source.Price);
        }
    }
}
