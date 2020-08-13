using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class TierPriceType : ObjectGraphType<TierPrice>
    {
        public TierPriceType()
        {
            Field<MoneyType>("price", resolve: context => context.Source.Price);
            Field<MoneyType>("priceWithTax", resolve: context => context.Source.PriceWithTax);
            Field<LongGraphType>("quantity", resolve: context => context.Source.Quantity);
        }
    }
}
