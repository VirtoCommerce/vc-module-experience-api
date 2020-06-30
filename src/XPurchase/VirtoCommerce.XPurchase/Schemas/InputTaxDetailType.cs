using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputTaxDetailType : InputObjectGraphType<TaxDetail>
    {
        public InputTaxDetailType()
        {
            Field<InputMoneyType>("amount", resolve: context => context.Source.Amount.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("price", resolve: context => context.Source.Rate.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("rate", resolve: context => context.Source.Rate.ToMoney(context.GetCart().Currency));
            Field<StringGraphType>("name", resolve: context => context.Source.Name);
        }
    }
}
