using GraphQL.Types;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class TaxLineType : ObjectGraphType<TaxLine>
    {
        public TaxLineType()
        {
            Name = "TaxLine";
            Field(d => d.Id);
            Field(d => d.Name);
            Field(d => d.Code);
            Field(d => d.TaxType);
            Field(d => d.Name);
            Field<InterfaceGraphType>("Quantity", resolve: context => context.Source.Quantity);
            Field<DecimalGraphType>("Amount", resolve: context => context.Source.Amount);
            Field<DecimalGraphType>("Price", resolve: context => context.Source.Price);
        }
    }
}
