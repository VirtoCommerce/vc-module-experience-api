using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class TaxRateType : ObjectGraphType<TaxRate>
    {
        public TaxRateType()
        {
            Name = "TaxRate";
            Field<ObjectGraphType<TaxLineType>>("Line", resolve: context => context.Source.Line);
            Field<DecimalGraphType>("Rate", resolve: context => context.Source.Rate);
            Field<DecimalGraphType>("PercentRate", resolve: context => context.Source.PercentRate);
            Field<TaxLineType>("Line", resolve: context => context.Source.Line);
            Field(d=>d.TaxProviderCode);
            Field<ListGraphType<TaxDetailType>>("TaxDetails", resolve: context => context.Source.TaxDetails);
        }
    }
}
