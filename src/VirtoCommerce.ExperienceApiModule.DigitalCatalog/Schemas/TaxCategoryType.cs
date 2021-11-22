using System.Linq;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class TaxCategoryType : ObjectGraphType<TaxCategory>
    {
        public TaxCategoryType()
        {
            Field<ListGraphType<TaxRateType>>("rates",
                "Tax rates",
                resolve: context => context.Source.Rates.ToList());
        }
    }
}
