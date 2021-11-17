using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class TierPriceType : ObjectGraphType<TierPrice>
    {
        public TierPriceType()
        {
            Field<MoneyType>("price",
                "Price",
                resolve: context => context.Source.Price);
            Field<MoneyType>("priceWithTax",
                "Price with tax",
                resolve: context => context.Source.PriceWithTax);
            Field<LongGraphType>("quantity",
                "Quantity",
                resolve: context => context.Source.Quantity);
        }
    }
}
