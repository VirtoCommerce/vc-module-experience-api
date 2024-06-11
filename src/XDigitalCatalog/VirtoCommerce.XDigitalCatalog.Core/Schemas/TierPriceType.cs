using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Core.Schemas
{
    public class TierPriceType : ObjectGraphType<TierPrice>
    {
        public TierPriceType()
        {
            Field<NonNullGraphType<MoneyType>>("price",
                "Price",
                resolve: context => context.Source.Price);
            Field<NonNullGraphType<MoneyType>>("priceWithTax",
                "Price with tax",
                resolve: context => context.Source.PriceWithTax);
            Field<NonNullGraphType<LongGraphType>>("quantity",
                "Quantity",
                resolve: context => context.Source.Quantity);
        }
    }
}
