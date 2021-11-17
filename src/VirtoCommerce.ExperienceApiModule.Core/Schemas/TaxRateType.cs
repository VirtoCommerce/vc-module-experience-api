using GraphQL.Types;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class TaxRateType : ObjectGraphType<TaxRate>
    {
        public TaxRateType()
        {
            Name = "TaxRate";

            Field<DecimalGraphType>("rate",
                "Tax rate",
                resolve: context => context.Source.Rate);
            Field<DecimalGraphType>("percentRate",
                "Tax rate percentage",
                resolve: context => context.Source.PercentRate);
            Field<TaxLineType>("line",
                "Tax line",
                resolve: context => context.Source.Line);
            Field(d => d.TaxProviderCode).Description("Tax provider code");
            Field<ListGraphType<TaxDetailType>>("taxDetails",
                "Tax details",
                resolve: context => context.Source.TaxDetails);
        }
    }
}
