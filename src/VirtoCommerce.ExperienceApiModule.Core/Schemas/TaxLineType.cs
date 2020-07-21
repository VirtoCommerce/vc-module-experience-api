using GraphQL.Types;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class TaxLineType : ObjectGraphType<TaxLine>
    {
        public TaxLineType()
        {
            Name = "TaxLine";

            Field(d => d.Id); // TODO: add desciption
            Field(d => d.Name); // TODO: add desciption
            Field(d => d.Code); // TODO: add desciption
            Field(d => d.TaxType); // TODO: add desciption
            Field(d => d.Name); // TODO: add desciption
            Field<InterfaceGraphType>("quantity", resolve: context => context.Source.Quantity);
            Field<DecimalGraphType>("amount", resolve: context => context.Source.Amount);
            Field<DecimalGraphType>("price", resolve: context => context.Source.Price);
        }
    }
}
