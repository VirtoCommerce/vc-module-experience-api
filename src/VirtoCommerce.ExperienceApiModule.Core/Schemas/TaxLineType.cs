using GraphQL.Types;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class TaxLineType : ObjectGraphType<TaxLine>
    {
        public TaxLineType()
        {
            Name = "TaxLine";

            Field(d => d.Id); // PT-1612: add desciption
            Field(d => d.Name); // PT-1612: add desciption
            Field(d => d.Code); // PT-1612: add desciption
            Field(d => d.TaxType); // PT-1612: add desciption
            Field<IntGraphType>("quantity", resolve: context => context.Source.Quantity);
            Field<DecimalGraphType>("amount", resolve: context => context.Source.Amount);
            Field<DecimalGraphType>("price", resolve: context => context.Source.Price);
        }
    }
}
