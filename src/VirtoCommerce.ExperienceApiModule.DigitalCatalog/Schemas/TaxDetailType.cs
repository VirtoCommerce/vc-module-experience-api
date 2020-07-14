using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Tax;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class TaxDetailType : ObjectGraphType<TaxDetail>
    {
        public TaxDetailType()
        {
            Name = "TaxDetail";

            Field(x => x.Name).Description("Tax detail name.");
            Field<DecimalGraphType>("amount", resolve: context => context.Source.Amount);
            Field<DecimalGraphType>("rate", resolve: context => context.Source.Rate);
        }
    }
}
