using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class TaxDetailType : ObjectGraphType<TaxDetail>
    {
        public TaxDetailType()
        {
            Field<ObjectGraphType<MoneyType>>("rate", resolve: context => context.Source.Rate);
            Field<ObjectGraphType<MoneyType>>("amount", resolve: context => context.Source.Amount);
            Field<ObjectGraphType<MoneyType>>("name", resolve: context => context.Source.Name);
            Field<ObjectGraphType<MoneyType>>("price", resolve: context => context.Source.Price);
        }
    }
}
