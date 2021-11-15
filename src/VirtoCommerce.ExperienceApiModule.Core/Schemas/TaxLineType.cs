using GraphQL.Types;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class TaxLineType : ObjectGraphType<TaxLine>
    {
        public TaxLineType()
        {
            Name = "TaxLine";

            Field(d => d.Id).Description("Tax line Id");
            Field(d => d.Name).Description("Tax name");
            Field(d => d.Code).Description("Tax code");
            Field(d => d.TaxType).Description("Tax type");
            Field<IntGraphType>("quantity",
                "Tax quantity",
                resolve: context => context.Source.Quantity);
            Field<DecimalGraphType>("amount",
                "Tax amount",
                resolve: context => context.Source.Amount);
            Field<DecimalGraphType>("price",
                "Tax price",
                resolve: context => context.Source.Price);
        }
    }
}
