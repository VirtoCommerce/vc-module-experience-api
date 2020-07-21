using GraphQL.Types;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class TaxRateType : ObjectGraphType<TaxRate>
    {
        public TaxRateType()
        {
            Name = "TaxRate";

            Field<ObjectGraphType<TaxLineType>>("line", resolve: context => context.Source.Line);
            Field<DecimalGraphType>("rate", resolve: context => context.Source.Rate);
            Field<DecimalGraphType>("percentRate", resolve: context => context.Source.PercentRate);
            Field<TaxLineType>("line", resolve: context => context.Source.Line);
            Field(d => d.TaxProviderCode).Description("Tax provider code");
            Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
        }
    }
}
