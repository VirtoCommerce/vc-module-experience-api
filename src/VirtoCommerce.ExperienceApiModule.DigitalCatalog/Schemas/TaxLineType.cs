using GraphQL.Types;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class TaxLineType : ObjectGraphType<TaxLine>
    {
        public TaxLineType() // TODO: remove this
        {
            Name = "TaxLine";

            Field(d => d.Id);
            Field(d => d.Name);
            Field(d => d.Code);
            Field(d => d.TaxType);
            Field(d => d.Name);
            Field<InterfaceGraphType>("quantity", resolve: context => context.Source.Quantity);
            Field<DecimalGraphType>("amount", resolve: context => context.Source.Amount);
            Field<DecimalGraphType>("price", resolve: context => context.Source.Price);
        }
    }
}
