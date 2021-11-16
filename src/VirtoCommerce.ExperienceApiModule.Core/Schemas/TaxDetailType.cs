using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class TaxDetailType : ObjectGraphType<TaxDetail>
    {
        public TaxDetailType()
        {
            Name = "TaxDetail";

            Field(x => x.Name).Description("Tax detail name");
            Field<DecimalGraphType>("amount",
                "Tax amount details",
                resolve: context => context.Source.Amount);
            Field<DecimalGraphType>("rate",
                "Tax rate details",
                resolve: context => context.Source.Rate);
        }
    }
}
